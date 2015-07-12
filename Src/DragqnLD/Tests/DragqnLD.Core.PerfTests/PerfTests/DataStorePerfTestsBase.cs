using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Implementations;
using DragqnLD.Core.UnitTests;
using DragqnLD.Core.UnitTests.Utils;
using JsonLD.Core;
using Raven.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace DragqnLD.Core.PerfTests.PerfTests
{
    [Trait("Category", "Perf test")]
    public class PerfDataStoreFixture : DataStoreFixture
    {
        public readonly ExpandedJsonLDDataFormatter Formatter;
        public readonly List<Uri> IngredientsIds = new List<Uri>();
        public readonly List<Uri> MedicinalProductsIds = new List<Uri>();

        public PerfDataStoreFixture()
        {
            Formatter = new ExpandedJsonLDDataFormatter();

            DocumentStore.RegisterListener(new NoStaleQueriesListener());
            DocumentStore.RegisterListener(new NoTrackingQueriesListener());
            DocumentStore.RegisterListener(new NoCachingQueriesListener());

            var ingredientsTask = StoreTestData(TestDataConstants.IngredientsFolder,
                TestDataConstants.IngredientsQueryDefinitionId,
                TestDataConstants.IngredientsNamespacePrefix,
                IngredientsIds);
            var medicinalProductsTask = StoreTestData(TestDataConstants.MedicinalProductsFolder,
                TestDataConstants.MedicinalProductQueryDefinitionId,
                TestDataConstants.MedicinalProductNamespacePrefix,
                MedicinalProductsIds);

            Task.WaitAll(ingredientsTask, medicinalProductsTask);
        }

        private async Task StoreTestData(string inputFolder, string queryId, string idPrefix, List<Uri> documentIdsToFill)
        {
            var documents = ConstructResultsForFolder(inputFolder, queryId, idPrefix).ToList();
            documentIdsToFill.AddRange(documents.Select(cr => cr.DocumentId));

            await RavenDataStore.BulkStoreDocuments(documents);
        }
        private IEnumerable<ConstructResult> ConstructResultsForFolder(string inputFolder, string queryId, string idPrefix)
        {
            var inputDirectoryInfo = new DirectoryInfo(inputFolder);
            var inputFiles = inputDirectoryInfo.GetFiles();

            foreach (FileInfo inputFile in inputFiles)
            {
                var idWithoutNamespace = inputFile.Name.Substring(0, inputFile.Name.Length - ".json".Length);
                var id = idPrefix + idWithoutNamespace;
                var uriId = new Uri(id);
                RavenJObject document;
                using (var input = new StreamReader(inputFile.FullName))
                using (var writer = new StringWriter())
                {
                    PropertyMappings mappings;
                    //todo: context?
                    var context = ContextTestHelper.EmptyContext();
                    Formatter.Format(input, writer, id, context, out mappings);
                    document = RavenJObject.Parse(writer.ToString());
                }
                yield return new ConstructResult { QueryId = queryId, DocumentId = uriId, Document = new Document { Content = document } };
            }

        }
    }

    [CollectionDefinition("Perf tests")]
// ReSharper disable once UnusedMember.Global
    public class PerfTestCollection : ICollectionFixture<PerfDataStoreFixture>
    {

    }

    [Trait("Category", "Perf test")]
    [Collection("Perf tests")]
    public abstract class DataStorePerfTestsBase : DataStoreTestsBase, IDisposable
    {
        private const bool ListenToGcNotification = false;
        
        private class GcNotificationListener : IDisposable
        {
            private Thread startpolling;
            private readonly ITestOutputHelper _output;

            public GcNotificationListener(ITestOutputHelper output)
            {
                _output = output;
            }

            public void Start()
            {
                GC.RegisterForFullGCNotification(10, 10);

                startpolling = new Thread(() =>
                {
                    _output.WriteLine("GC listening");
                    while (true)
                    {
                        // Check for a notification of an approaching collection.
                        GCNotificationStatus s = GC.WaitForFullGCApproach();
                        if (s == GCNotificationStatus.Succeeded)
                        {
                            //Call event

                            _output.WriteLine("GC is about to begin");
                            GC.Collect();

                        }
                        else if (s == GCNotificationStatus.Canceled)
                        {
                            _output.WriteLine("GC wait for full gc Approach cancelled");
                        }
                        else if (s == GCNotificationStatus.Timeout)
                        {
                            _output.WriteLine("GC wait for full gc Approach timout");
                        }

                        // Check for a notification of a completed collection.
                        s = GC.WaitForFullGCComplete();
                        if (s == GCNotificationStatus.Succeeded)
                        {
                            //Call event
                            _output.WriteLine("GC has ended");
                        }
                        else if (s == GCNotificationStatus.Canceled)
                        {
                            _output.WriteLine("GC wait for full gc complete cancelled");
                        }
                        else if (s == GCNotificationStatus.Timeout)
                        {
                            _output.WriteLine("GC wait for full gc complete timeout");
                        }
                    }
// ReSharper disable once FunctionNeverReturns
                });

                startpolling.Start();
            }
            
            public void Dispose()
            {
                GC.CancelFullGCNotification();
                _output.WriteLine("GC notif cancelled");
                startpolling.Abort();
                startpolling = null;
            }
        }
        
        private GcNotificationListener _gcListener;
        protected ExpandedJsonLDDataFormatter Formatter;
        protected List<Uri> IngredientsIds = new List<Uri>();
        protected List<Uri> MedicinalProductsIds = new List<Uri>();

        protected DataStorePerfTestsBase(ITestOutputHelper output, PerfDataStoreFixture perfDataStoreFixture)
            : base(output, perfDataStoreFixture)
        {
            Formatter = perfDataStoreFixture.Formatter;
            IngredientsIds = perfDataStoreFixture.IngredientsIds;
            MedicinalProductsIds = perfDataStoreFixture.MedicinalProductsIds;

// ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (ListenToGcNotification)
#pragma warning disable 162
            {
                _gcListener = new GcNotificationListener(output);
                _gcListener.Start();
            }
#pragma warning restore 162
        }

        public void Profile(string description, int iterations, Action func,
            Action withFirstRun = null)
        {
            TestUtilities.Profile(description, iterations, Output, func, withFirstRun);
        }

        public async Task Profile(string description, int iterations, Func<Task> func,
            Func<Task> withFirstRun = null)
        {
            await TestUtilities.Profile(description, iterations, Output, func, withFirstRun);
        }

        public void Dispose()
        {
// ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (ListenToGcNotification)
#pragma warning disable 162
            {
                _gcListener.Dispose();
                _gcListener = null;
            }
#pragma warning restore 162
        }
    }
}
