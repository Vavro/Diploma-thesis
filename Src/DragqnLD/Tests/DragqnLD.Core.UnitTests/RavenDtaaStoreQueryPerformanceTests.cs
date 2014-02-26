using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Implementations;
using DragqnLD.Core.Implementations.Utils;
using Raven.Json.Linq;
using Raven.Tests.Helpers;
using Xunit;

namespace DragqnLD.Core.UnitTests
{
    public class RavenDtaaStoreQueryPerformanceTests : DataStoreTestsBase
    {
        private readonly ExpandedJsonLDDataFormatter _formatter;

        public RavenDtaaStoreQueryPerformanceTests()
        {
            _formatter = new ExpandedJsonLDDataFormatter();
        }

        [Fact]
        public async Task QueryRootExactDescriptionIngredientProperty()
        {
            var queryId = "QueryDefinitions/1";
            var inputFolder = TestDataFolders.Ingredients;
            var idPrefix = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/";
            var documents = ConstructResultsForFolder(inputFolder, queryId, idPrefix);

            var searchedDescritpion =
                @"""Analgesic antipyretic derivative of acetanilide. It has weak anti-inflammatory properties and is used as a common analgesic, but may cause liver, blood cell, and kidney damage.     """;
            var searchedId = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115";

            await _ravenDataStore.BulkStoreDocuments(documents);

            _documentStore.Conventions.ShouldCacheRequest = should => false;
            TestUtilities.Profile("Query exact Description", 100, () =>
            {
                var task =  _ravenDataStore.QueryDocumentProperties(queryId,
                        @"http://linked.opendata.cz/ontology/drug-encyclopedia/description,@value".AsCondition(searchedDescritpion));
                task.Wait();
            });
            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
        }

        [Fact]
        public async Task QueryRootExactDescriptionMedicianalProductProperty()
        {
            var queryId = "QueryDefinitions/1";
            var inputFolder = TestDataFolders.MedicinalProducts;
            var idPrefix = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/";
            var documents = ConstructResultsForFolder(inputFolder, queryId, idPrefix);

            var searchedDescritpion =
                @"""Analgesic antipyretic derivative of acetanilide. It has weak anti-inflammatory properties and is used as a common analgesic, but may cause liver, blood cell, and kidney damage.     """;
            var searchedId = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115";

            await _ravenDataStore.BulkStoreDocuments(documents);

            _documentStore.Conventions.ShouldCacheRequest = should => false;
            TestUtilities.Profile("Query exact Description", 100, () =>
            {
                var task = _ravenDataStore.QueryDocumentProperties(queryId,
                        @"http://linked.opendata.cz/ontology/drug-encyclopedia/description,@value".AsCondition(searchedDescritpion));
                task.Wait();
            });
            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
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
                    _formatter.Format(input, writer, id, out mappings);
                    document = RavenJObject.Parse(writer.ToString());
                }
                yield return new ConstructResult() {QueryId = queryId, DocumentId = uriId, Document = new Document() {Content = document}};
            }

        }
    }
}
