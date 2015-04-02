using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Security.Principal;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Implementations;
using DragqnLD.Core.UnitTests.Utils;
using Raven.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace DragqnLD.Core.UnitTests.PerfTests
{
    public class PerfDataStoreFixture : DataStoreFixture
    {
        public readonly ExpandedJsonLDDataFormatter Formatter;
        public readonly List<Uri> IngredientsIds = new List<Uri>();
        public readonly List<Uri> MedicinalProductsIds = new List<Uri>(); 

        public PerfDataStoreFixture()
        {
            Formatter = new ExpandedJsonLDDataFormatter();

            DocumentStore.RegisterListener(new NoStaleQueriesListener())
                .RegisterListener(new NoTrackingQueriesListener())
                .RegisterListener(new NoCachingQueriesListener());

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
                    Formatter.Format(input, writer, id, out mappings);
                    document = RavenJObject.Parse(writer.ToString());
                }
                yield return new ConstructResult { QueryId = queryId, DocumentId = uriId, Document = new Document { Content = document } };
            }

        }
    }

    [CollectionDefinition("Perf tests")]
    public class PerfTestCollection : ICollectionFixture<PerfDataStoreFixture>
    {
        
    }

    [Collection("Perf tests")]
    public abstract class DataStorePerfTestsBase : DataStoreTestsBase
    {
        protected ExpandedJsonLDDataFormatter Formatter;
        protected List<Uri> IngredientsIds = new List<Uri>();
        protected List<Uri> MedicinalProductsIds = new List<Uri>(); 

        protected DataStorePerfTestsBase(ITestOutputHelper output, PerfDataStoreFixture perfDataStoreFixture) : base(output, perfDataStoreFixture)
        {
            Formatter = perfDataStoreFixture.Formatter;
            IngredientsIds= perfDataStoreFixture.IngredientsIds;
            MedicinalProductsIds = perfDataStoreFixture.MedicinalProductsIds;
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

    }
}
