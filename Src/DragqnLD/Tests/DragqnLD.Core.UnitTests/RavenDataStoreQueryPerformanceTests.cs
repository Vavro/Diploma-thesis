﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Implementations;
using DragqnLD.Core.Implementations.Utils;
using DragqnLD.Core.UnitTests.Utils;
using Raven.Abstractions.Indexing;
using Raven.Json.Linq;
using Raven.Tests.Helpers;
using Xunit;
using Xunit.Extensions;

namespace DragqnLD.Core.UnitTests
{
    public class RavenDataStoreQueryPerformanceTests : DataStoreTestsBase
    {
        private readonly ExpandedJsonLDDataFormatter _formatter;

        public RavenDataStoreQueryPerformanceTests()
        {
            _formatter = new ExpandedJsonLDDataFormatter();
            _documentStore.RegisterListener(new NoStaleQueriesListener());
        }

        [Theory]
        [InlineData("QueryDefinitions/1", 
            TestDataFolders.Ingredients, 
            @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/",
            @"http://linked.opendata.cz/ontology/drug-encyclopedia/description,@value",
            @"""Analgesic antipyretic derivative of acetanilide. It has weak anti-inflammatory properties and is used as a common analgesic, but may cause liver, blood cell, and kidney damage.     """,
            1)]
        [InlineData("QueryDefinitions/2",
            TestDataFolders.MedicinalProducts,
            @"http://linked.opendata.cz/resource/sukl/medicinal-product/",
            @"http://linked.opendata.cz/ontology/drug-encyclopedia/title,@value",
            @"""ABILIFY 7,5 MG/ML""",
            1)]
        [InlineData("QueryDefinition/1",
            TestDataFolders.Ingredients,
            @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/",
            @"http://linked.opendata.cz/ontology/drug-encyclopedia/hasPregnancyCategory,",
            @"""http://linked.opendata.cz/resource/fda-spl/pregnancy-category/C""",
            110)]
        //todo: slow perf of hasPregnancy property query could be because values are really close - figure this out
        public async Task QueryExactPropertyValueProperty(string queryId, string inputFolder, string idPrefix, string searchedProperty, string searchedValue, int expectedResultCount)
        {
            var documents = ConstructResultsForFolder(inputFolder, queryId, idPrefix);

            await _ravenDataStore.BulkStoreDocuments(documents);

            _documentStore.Conventions.ShouldCacheRequest = should => false;
            TestUtilities.Profile(
                String.Format("Query exact property value \n in {0} \n property {1} \n value {2} \n expected result count {3}", idPrefix, searchedProperty, searchedValue, expectedResultCount), 
                100, 
                () =>
                    {
                        var task = _ravenDataStore.QueryDocumentProperties(queryId,
                                searchedProperty.AsCondition(searchedValue));
                        var result = task.Result;
                        Assert.Equal(expectedResultCount, result.Count());
                    });
            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
        }
        //todo: add test for has this action and this product
        //todo: test for has action but not pregnancy C
        //todo: test fuzzy search title
        //todo: fulltext search on description fields?
        //todo: all ingrediences of medicinal product have pregnancy category C or better --- i.e. cannot be D or X
        
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

        //todo: add test for has these two ingrediets or these two Pharmacological actions, MayTreat - test more variants
        [Fact]
        public async Task QueryTwoSpecificPropertyValuesInChildrenCollections()
        {
            var queryId = "QueryDefinition/1";
            var inputFolder = TestDataFolders.Ingredients;
            var idPrefix = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/";
            var documents = ConstructResultsForFolder(inputFolder, queryId, idPrefix);

            await _ravenDataStore.BulkStoreDocuments(documents);

            //default dynamic index assumes that each object in hierarchy is one target for query, so it doesn't support multiple Values from two different objects, have to write index manually
            var indexName = "HasPharmalogicalActionIndex";
            var indexForValuesFromMultipleChildren =
                @"from doc in docs
let entityName = doc[""@metadata""][""Raven-Entity-Name""]
where entityName == ""QueryDefinition/1""
select new { http___linked_opendata_cz_ontology_drug_encyclopedia_hasPharmacologicalAction_http___linked_opendata_cz_ontology_drug_encyclopedia_title__value = ((IEnumerable<dynamic>)doc.http___linked_opendata_cz_ontology_drug_encyclopedia_hasPharmacologicalAction).DefaultIfEmpty().Select( c => ((IEnumerable<dynamic>)c.http___linked_opendata_cz_ontology_drug_encyclopedia_title).DefaultIfEmpty().Select( d => d._value)),
_metadata_Raven_Entity_Name = doc[""@metadata""][""Raven-Entity-Name""]}";

            await _documentStore.AsyncDatabaseCommands.PutIndexAsync(indexName,
                new IndexDefinition() {Map = indexForValuesFromMultipleChildren}, true);

            //had to escape "," for "_" as this is defined by the index
            var property =
                "http://linked.opendata.cz/ontology/drug-encyclopedia/hasPharmacologicalAction_http://linked.opendata.cz/ontology/drug-encyclopedia/title_@value";
            var searchedValue = @"(+""Analgesics, non-narcotic"" +""Antipyretics"")";

            TestUtilities.Profile("HasPharmalogical action Analgesics, non-narcotic and Antipyretics", 100, async () =>
            {
                var result = await _ravenDataStore.QueryDocumentProperties(queryId, indexName, property.AsCondition(searchedValue));
                Assert.Equal(1, result.Count());
                Assert.Equal(@"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115",
                    result.First().ToString());
            });
            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
        }

        //todo: add starts with test - i.e. title - APO*
        [Fact]
        public async Task QueryStartingWith()
        {
            var queryId = "QueryDefinition/2";
            var inputFolder = TestDataFolders.MedicinalProducts;
            var idPrefix = @"http://linked.opendata.cz/resource/sukl/medicinal-product/";
            var documents = ConstructResultsForFolder(inputFolder, queryId, idPrefix);

            await _ravenDataStore.BulkStoreDocuments(documents);

            var propertyName = @"http://linked.opendata.cz/ontology/drug-encyclopedia/title,@value";
            var searchedValue = "APO*";
            TestUtilities.Profile("Medicinal product Starts with 'APO' ", 100, async () =>
            {
                var result =
                    await _ravenDataStore.QueryDocumentProperties(queryId, propertyName.AsCondition(searchedValue));
                Assert.Equal(12, result.Count());
            });

            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
        }
    }
}