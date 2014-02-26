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
using Xunit.Extensions;

namespace DragqnLD.Core.UnitTests
{
    public class RavenDtaaStoreQueryPerformanceTests : DataStoreTestsBase
    {
        private readonly ExpandedJsonLDDataFormatter _formatter;

        public RavenDtaaStoreQueryPerformanceTests()
        {
            _formatter = new ExpandedJsonLDDataFormatter();
        }

        [Theory]
        [InlineData("QueryDefinitions/1", 
            TestDataFolders.Ingredients, 
            @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/",
            @"http://linked.opendata.cz/ontology/drug-encyclopedia/description,@value",
            @"""Analgesic antipyretic derivative of acetanilide. It has weak anti-inflammatory properties and is used as a common analgesic, but may cause liver, blood cell, and kidney damage.     """)]
        [InlineData("QueryDefinitions/2",
            TestDataFolders.MedicinalProducts,
            @"http://linked.opendata.cz/resource/sukl/medicinal-product/",
            @"http://linked.opendata.cz/ontology/drug-encyclopedia/title,@value",
            @"""ABILIFY 7,5 MG/ML""")]
        public async Task QueryExactPropertyValueProperty(string queryId, string inputFolder, string idPrefix, string searchedProperty, string searchedValue)
        {
            var documents = ConstructResultsForFolder(inputFolder, queryId, idPrefix);

            await _ravenDataStore.BulkStoreDocuments(documents);

            _documentStore.Conventions.ShouldCacheRequest = should => false;
            TestUtilities.Profile(
                String.Format("Query exact property value \n in {0} \n property {1} \n value {2}", idPrefix, searchedProperty, searchedValue), 
                100, 
                () =>
                    {
                        var task =  _ravenDataStore.QueryDocumentProperties(queryId,
                                searchedProperty.AsCondition(searchedValue));
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
