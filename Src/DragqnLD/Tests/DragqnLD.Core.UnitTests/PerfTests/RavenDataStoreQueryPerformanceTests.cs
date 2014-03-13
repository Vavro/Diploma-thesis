using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Implementations;
using DragqnLD.Core.Implementations.Utils;
using DragqnLD.Core.UnitTests.PerfTests;
using DragqnLD.Core.UnitTests.Utils;
using Raven.Abstractions.Extensions;
using Raven.Abstractions.Indexing;
using Raven.Json.Linq;
using Raven.Tests.Helpers;
using Xunit;
using Xunit.Extensions;

namespace DragqnLD.Core.UnitTests
{
    public class RavenDataStoreQueryPerformanceTests : DataStorePerfTestsBase
    {
        [Theory]
        [InlineData(TestDataConstants.IngredientsQueryDefinitionId, @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115")]
        [InlineData(TestDataConstants.IngredientsQueryDefinitionId, @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0006099")]
        [InlineData(TestDataConstants.MedicinalProductQueryDefinitionId, @"http://linked.opendata.cz/resource/sukl/medicinal-product/ABSEAMED-3000-IU-0-3-ML")]
        [InlineData(TestDataConstants.MedicinalProductQueryDefinitionId, @"http://linked.opendata.cz/resource/sukl/medicinal-product/BUPAINX-0-4-MG")]
        public void GetById(string queryId, string documentId)
        {
            var id = new Uri(documentId);
            TestUtilities.Profile(
                String.Format("GetById, queryId: {0}, id: {1}", queryId, documentId),
                1000,
                async () =>
                {
                    var document = await _ravenDataStore.GetDocument(queryId, id);
                    Assert.NotNull(document.Content);
                }
            );
        }

        [Theory]
        [InlineData(TestDataConstants.IngredientsQueryDefinitionId,
            TestDataConstants.IngredientsFolder,
            TestDataConstants.IngredientsNamespacePrefix,
            TestDataConstants.PropertyNameIngredientsDescription,
            @"""Analgesic antipyretic derivative of acetanilide. It has weak anti-inflammatory properties and is used as a common analgesic, but may cause liver, blood cell, and kidney damage.     """,
            1)]
        [InlineData(TestDataConstants.MedicinalProductQueryDefinitionId,
            TestDataConstants.MedicinalProductsFolder,
            TestDataConstants.MedicinalProductNamespacePrefix,
            TestDataConstants.PropertyNameMedicinalProductsTitle,
            @"""ABILIFY 7,5 MG/ML""",
            1)]
        [InlineData(TestDataConstants.IngredientsQueryDefinitionId,
            TestDataConstants.IngredientsFolder,
            TestDataConstants.IngredientsNamespacePrefix,
            TestDataConstants.PropertyNameIngredientsPregnancyCategory,
            @"""http://linked.opendata.cz/resource/fda-spl/pregnancy-category/C""",
            110)]
        //todo: slow perf of hasPregnancy property query could be because values are really close - figure this out
        public void QueryExactPropertyValueProperty(string queryId, string inputFolder, string idPrefix, string searchedProperty, string searchedValue, int expectedResultCount)
        {
            TestUtilities.Profile(
                String.Format("Query exact property value \n in {0} \n property {1} \n value {2} \n expected result count {3}", idPrefix, searchedProperty, searchedValue, expectedResultCount),
                100,
                async () =>
                {
                    var result = await _ravenDataStore.QueryDocumentProperties(queryId,
                            searchedProperty.AsCondition(searchedValue));
                    Assert.Equal(expectedResultCount, result.Count());
                });
            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
        }

        //todo: add test for has these two ingrediets or these two Pharmacological actions, MayTreat - test more variants
        [Fact]
        public void QueryTwoSpecificPropertyValuesInChildrenCollections()
        {
            var queryId = TestDataConstants.IngredientsQueryDefinitionId;

            //default dynamic index assumes that each object in hierarchy is one target for query, so it doesn't support multiple Values from two different objects, have to write index manually
            var indexName = "HasPharmalogicalActionIndex";
            var indexForValuesFromMultipleChildren =
                @"from doc in docs
let entityName = doc[""@metadata""][""Raven-Entity-Name""]
where entityName == ""QueryDefinitions/1""
select new { http___linked_opendata_cz_ontology_drug_encyclopedia_hasPharmacologicalAction_http___linked_opendata_cz_ontology_drug_encyclopedia_title__value = ((IEnumerable<dynamic>)doc.http___linked_opendata_cz_ontology_drug_encyclopedia_hasPharmacologicalAction).DefaultIfEmpty().Select( c => ((IEnumerable<dynamic>)c.http___linked_opendata_cz_ontology_drug_encyclopedia_title).DefaultIfEmpty().Select( d => d._value)),
_metadata_Raven_Entity_Name = doc[""@metadata""][""Raven-Entity-Name""]}";
            

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
            }, async () =>
            {
                await _documentStore.AsyncDatabaseCommands.PutIndexAsync(indexName,
                    new IndexDefinition { Map = indexForValuesFromMultipleChildren }, true);
            });
            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
        }

        //todo: add starts with test - i.e. title - APO*
        [Fact]
        public void QueryStartingWith()
        {
            var queryId = TestDataConstants.MedicinalProductQueryDefinitionId;

            var propertyName = TestDataConstants.PropertyNameMedicinalProductsTitle;
            var searchedValue = "APO*";
            TestUtilities.Profile("Medicinal product Starts with 'APO' ", 100, async () =>
            {
                var result =
                    await _ravenDataStore.QueryDocumentProperties(queryId, propertyName.AsCondition(searchedValue));
                Assert.Equal(12, result.Count());
            });

            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
        }

        //todo: test for ingredient with maytreat and specific pregnancy category
        [Fact]
        public void IngredientWithMayTreatAndPregnancyCategory()
        {
            var searchedPregnancyCategory = @"""http://linked.opendata.cz/resource/fda-spl/pregnancy-category/A""";
            var searchedMayTreatTitle = @"Pelagra";

            var expectedId = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0014807".ToLower();

            TestUtilities.Profile(
                String.Format("Searching for ingredient with \n MayTreat : {0}, \n PregnancyCategory : {1}", searchedMayTreatTitle, searchedPregnancyCategory),
                100,
                async () =>
                {
                    var result = await _ravenDataStore.QueryDocumentProperties(TestDataConstants.IngredientsQueryDefinitionId,
                        TestDataConstants.PropertyNameIngredientMayTreat.AsCondition(searchedMayTreatTitle),
                        TestDataConstants.PropertyNameIngredientPregnancyCategory.AsCondition(searchedPregnancyCategory));

                    Assert.Equal(1, result.Count());
                    Assert.Equal(expectedId, result.First().AbsoluteUri);
                });
        }

        //todo: test fuzzy search title
        [Fact]
        public void FuzzySearchTitle()
        {
            var queryId = TestDataConstants.MedicinalProductQueryDefinitionId;

            //todo: create index with standart analyzer so that just name works for fuzzy search

            var propertyNameMedicalProductsTitleEscaped =
                "http___linked_opendata_cz_ontology_drug_encyclopedia_title__value";

            var indexName = "FuzzyMedicalProductTitle";
            var indexDefinition =
                @"from doc in docs
let entityName = doc[""@metadata""][""Raven-Entity-Name""]
where entityName == ""QueryDefinitions/2""
select new { http___linked_opendata_cz_ontology_drug_encyclopedia_title__value = ((IEnumerable<dynamic>)doc.http___linked_opendata_cz_ontology_drug_encyclopedia_title).DefaultIfEmpty().Select( d => d._value),
_metadata_Raven_Entity_Name = doc[""@metadata""][""Raven-Entity-Name""]}";

            //RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
            
            var searchedTitle = "ARXTRA~";
            var expectedResultCount = 6;

            TestUtilities.Profile(
                String.Format("Fuzzy search for {0}, expected results {1}", searchedTitle, expectedResultCount),
                100,
                async () =>
                {
                    var result = await _ravenDataStore.QueryDocumentProperties(queryId, indexName,
                        propertyNameMedicalProductsTitleEscaped.AsCondition(searchedTitle));

                    Assert.Equal(expectedResultCount, result.Count());
                }, async () =>
                {
                    await _documentStore.AsyncDatabaseCommands.PutIndexAsync(indexName,
                        new IndexDefinition
                        {
                            Map = indexDefinition,
                            Analyzers =
                                new Dictionary<string, string>()
                        {
                            {propertyNameMedicalProductsTitleEscaped, TestDataConstants.AnalyzerLuceneStandard}
                        }
                        }, true);
                });
            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
        }

        //todo: fulltext search on description fields?
        [Fact]
        public void FullTextSearchDescription()
        {
            var queryId = TestDataConstants.IngredientsQueryDefinitionId;

            var propertyNameMedicalProductsDescriptionEscaped = @"http___linked_opendata_cz_ontology_drug_encyclopedia_description__value";

            var indexName = "FullTextMedicalProductDescription";
            var indexDefinition =
                @"from doc in docs
let entityName = doc[""@metadata""][""Raven-Entity-Name""]
where entityName == ""QueryDefinitions/1""
select new { http___linked_opendata_cz_ontology_drug_encyclopedia_description__value = ((IEnumerable<dynamic>)doc.http___linked_opendata_cz_ontology_drug_encyclopedia_description).DefaultIfEmpty().Select( d => d._value),
_metadata_Raven_Entity_Name = doc[""@metadata""][""Raven-Entity-Name""]}";

            var searchedText = "(semisynthetic ergotamine alkaloid)";
            var expectedResultCount = 13;

            TestUtilities.Profile("Fullsearch on description of ingredients", 100, async () =>
            {
                var result =
                    await
                        _ravenDataStore.QueryDocumentProperties(queryId, indexName,
                            propertyNameMedicalProductsDescriptionEscaped.AsCondition(searchedText));

                Assert.Equal(expectedResultCount, result.Count());
            }, async () =>
            {
                await _documentStore.AsyncDatabaseCommands.PutIndexAsync(indexName,
                    new IndexDefinition()
                    {
                        Map = indexDefinition,
                        Analyzers =
                            new Dictionary<string, string>()
                        {
                            {propertyNameMedicalProductsDescriptionEscaped, TestDataConstants.AnalyzerLuceneStandard}
                        }
                    }, true);
            });

            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
        }

        //todo: Medicinal Product with atc concept and not having ingredience with contraindication
        [Fact]
        public void MedicinalProductWithAtcNotContraindication()
        {
            var queryId = TestDataConstants.MedicinalProductQueryDefinitionId;

            var escapedLuceneQuery =
@"http___linked_opendata_cz_ontology_drug_encyclopedia_hasATCConcept_http___www_w3_org_2004_02_skos_core_broaderTransitive_http___purl_org_dc_terms_title__value: ""ANTIANEMIC PREPARATIONS""
AND
-http___linked_opendata_cz_ontology_drug_encyclopedia_hasActiveIngredient_http___linked_opendata_cz_ontology_drug_encyclopedia_contraindicatedWith_http___linked_opendata_cz_ontology_drug_encyclopedia_title__value: ""Hypertension""";

            var indexName = "MPWithAtcAndContraindication";
            var indexDefinition =
@"from doc in docs
where doc[""@metadata""][""Raven-Entity-Name""] == ""QueryDefinitions/2""
select new { 
http___linked_opendata_cz_ontology_drug_encyclopedia_hasActiveIngredient_http___linked_opendata_cz_ontology_drug_encyclopedia_contraindicatedWith_http___linked_opendata_cz_ontology_drug_encyclopedia_title__value 
= ((IEnumerable<dynamic>)doc.http___linked_opendata_cz_ontology_drug_encyclopedia_hasActiveIngredient).DefaultIfEmpty().Select(x => ((IEnumerable<dynamic>)x.http___linked_opendata_cz_ontology_drug_encyclopedia_contraindicatedWith).DefaultIfEmpty().Select(y => ((IEnumerable<dynamic>)y.http___linked_opendata_cz_ontology_drug_encyclopedia_title).DefaultIfEmpty().Select(z => z._value))),
http___linked_opendata_cz_ontology_drug_encyclopedia_hasATCConcept_http___www_w3_org_2004_02_skos_core_broaderTransitive_http___purl_org_dc_terms_title__value 
= ((IEnumerable<dynamic>)doc.http___linked_opendata_cz_ontology_drug_encyclopedia_hasATCConcept).DefaultIfEmpty().Select(x => ((IEnumerable<dynamic>)x.http___www_w3_org_2004_02_skos_core_broaderTransitive).DefaultIfEmpty().Select(y => ((IEnumerable<dynamic>)y.http___purl_org_dc_terms_title).DefaultIfEmpty().Select(z => z._value))),
_metadata_Raven_Entity_Name = doc[""@metadata""][""Raven-Entity-Name""]}";

            
            TestUtilities.Profile(
                "Medicinal product, with broader atc concept \"Antianemic preparations\" but not having contraindicated with \"hypertension\" in active ingredients",
                100,
                async () =>
                {
                    var results =
                        await _ravenDataStore.QueryDocumentEscapedLuceneQuery(queryId, indexName, escapedLuceneQuery);
                    Assert.Equal(27, results.Count());
                }, async () =>
                {
                    await _documentStore.AsyncDatabaseCommands.PutIndexAsync(indexName, new IndexDefinition() { Map = indexDefinition }, true);
                });

            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);

        }
    }
}
