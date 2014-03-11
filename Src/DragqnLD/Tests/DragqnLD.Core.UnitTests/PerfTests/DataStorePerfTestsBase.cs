using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Implementations;
using DragqnLD.Core.UnitTests.Utils;
using Raven.Json.Linq;

namespace DragqnLD.Core.UnitTests.PerfTests
{
    public abstract class DataStorePerfTestsBase : DataStoreTestsBase
    {
        protected ExpandedJsonLDDataFormatter Formatter;
        protected List<Uri> IngredientsIds = new List<Uri>();
        protected List<Uri> MedicinalProductsIds = new List<Uri>(); 

        protected DataStorePerfTestsBase()
        {
            Init();
        }

        protected void Init()
        {
            Formatter = new ExpandedJsonLDDataFormatter();

            _documentStore.RegisterListener(new NoStaleQueriesListener())
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
            var documents = ConstructResultsForFolder(inputFolder, queryId, idPrefix);
            documentIdsToFill.AddRange(documents.Select(cr => cr.DocumentId));

            await _ravenDataStore.BulkStoreDocuments((IEnumerable<ConstructResult>) documents);
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
}
