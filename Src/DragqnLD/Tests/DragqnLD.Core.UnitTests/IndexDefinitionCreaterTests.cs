using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;
using DragqnLD.Core.UnitTests.Utils;
using Raven.Client.Embedded;
using Xunit;
using Xunit.Abstractions;

namespace DragqnLD.Core.UnitTests
{
    [Trait("Category", "Basic")]
    public class IndexDefinitionCreaterTests : TestsBase, IClassFixture<QueryStoreFixture>
    {
        private IndexDefinitionCreater _indexDefinitionCreater;
        private EmbeddableDocumentStore _documentStore;
        private QueryStore _queryStore;

        public IndexDefinitionCreaterTests(QueryStoreFixture qsFixture, ITestOutputHelper output) : base (output)
        {
            _indexDefinitionCreater = new IndexDefinitionCreater();
            _documentStore = qsFixture.DocumentStore;
            _queryStore = qsFixture.QueryStore;
        }

        [Fact]
        public void CanCreateSimpleIndex()
        {
            ConstructQueryAccessibleProperties propertyPaths;
            var ingredientsQd = PrepareIngredientQueryForIndexCreation(out propertyPaths);

            var propertiesToIndex = new DragqnLDIndexRequirements();
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { AbbreviatedName = "@id"});

            var indexDefiniton = _indexDefinitionCreater.CreateIndexDefinitionFor(ingredientsQd, propertyPaths, propertiesToIndex);
            const string expectedMap = @"from doc in docs
where doc[""@metadata""][""Raven-Entity-Name""] == ""Query/1""
select new { 
_id = doc._id,
_metadata_Raven_Entity_Name = doc[""@metadata""][""Raven-Entity-Name""]}";

            Assert.Equal(expectedMap, indexDefiniton.RavenMap);
            const string expectedName = "Query/1/_id";
            Assert.Equal(expectedName, indexDefiniton.Name);
        }

        [Fact]
        public void CanCreateComplexIndex()
        {
            ConstructQueryAccessibleProperties propertyPaths;
            var ingredientsQd = PrepareIngredientQueryForIndexCreation(out propertyPaths);

            var propertiesToIndex = new DragqnLDIndexRequirements();
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { AbbreviatedName = "@id" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() {AbbreviatedName = "title"});
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { AbbreviatedName = "hasMedicinalProduct.hasATCConcept.prefLabel" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { AbbreviatedName = "hasMedicinalProduct.hasATCConcept.notation" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { AbbreviatedName = "hasMechanismOfAction.@id" });

            var indexDefiniton = _indexDefinitionCreater.CreateIndexDefinitionFor(ingredientsQd, propertyPaths, propertiesToIndex);
            const string expectedMap = @"from doc in docs
where doc[""@metadata""][""Raven-Entity-Name""] == ""Query/1""
select new { 
_id = doc._id,
title = ((IEnumerable<dynamic>)doc.title).DefaultIfEmpty().Select(x0 => 
x0._value),
hasMedicinalProduct_hasATCConcept_prefLabel = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => 
((IEnumerable<dynamic>)x0.hasATCConcept).DefaultIfEmpty().Select(x1 => 
((IEnumerable<dynamic>)x1.prefLabel).DefaultIfEmpty().Select(x2 => 
x2._value))),
hasMedicinalProduct_hasATCConcept_notation = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => 
((IEnumerable<dynamic>)x0.hasATCConcept).DefaultIfEmpty().Select(x1 => 
((IEnumerable<dynamic>)x1.notation).DefaultIfEmpty().Select(x2 => 
x2))),
hasMechanismOfAction__id = ((IEnumerable<dynamic>)doc.hasMechanismOfAction).DefaultIfEmpty().Select(x0 => 
x0._id),
_metadata_Raven_Entity_Name = doc[""@metadata""][""Raven-Entity-Name""]}";

            Assert.Equal(expectedMap, indexDefiniton.RavenMap);
            const string expectedName = "Query/1/_id_title_hasMedicinalProduct_hasATCConcept_prefLabel_hasMedicinalProduct_hasATCConcept_notation_hasMechanismOfAction__id";
            Assert.Equal(expectedName, indexDefiniton.Name);
        }

        private static QueryDefinition PrepareIngredientQueryForIndexCreation(
            out ConstructQueryAccessibleProperties propertyPaths)
        {
            var ingredientsQd = TestQueries.TestQueryDefinitionIngredients;

            var constructQueryAnalyzer = new ConstructAnalyzer();
            var parsedQuery = ConstructAnalyzerHelper.ReplaceParamAndParseConstructQuery(ingredientsQd);
            var compactionContext = constructQueryAnalyzer.CreateCompactionContextForQuery(parsedQuery);
            propertyPaths = constructQueryAnalyzer.CreatePropertyPathsForQuery(parsedQuery, compactionContext);

            //add array info to property paths
            var inputDirectoryInfo = new DirectoryInfo(TestDataConstants.IngredientsFolder);
            var inputFiles = inputDirectoryInfo.GetFiles().Where(file => file.Name == "M0000115.json" || file.Name == "M0000171.json").ToList();

            string[] ids = new []
            {
                @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115",
                @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000171"
            };
            var writer = new StringWriter();
            var convertedContext = DataLoaderHelper.ConvertCompactionContext(compactionContext);
            for (int index = 0; index < inputFiles.Count; index++)
            {
                var inputFile = inputFiles[index];
                var output = ExpandedJsonLDDataFormatterTests.GetFormatted(inputFile.FullName, ids[index], writer,
                    convertedContext,
                    propertyPaths);
            }

            return ingredientsQd;
        }

        [Fact]
        public void CanProduceRightFieldString()
        {
            ConstructQueryAccessibleProperties propertyPaths;
            var ingredientsQd = PrepareIngredientQueryForIndexCreation(out propertyPaths);

            var fieldNameAndAccess = _indexDefinitionCreater.CreateIndexedFieldNameAndAccess(propertyPaths,
                new PropertyToIndex() {AbbreviatedName = "hasMedicinalProduct.hasATCConcept.prefLabel"});
            var createdLine = fieldNameAndAccess.Name + " = " + fieldNameAndAccess.Access;

            const string expectedLine = @"hasMedicinalProduct_hasATCConcept_prefLabel = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => 
((IEnumerable<dynamic>)x0.hasATCConcept).DefaultIfEmpty().Select(x1 => 
((IEnumerable<dynamic>)x1.prefLabel).DefaultIfEmpty().Select(x2 => 
x2._value)))";

            Assert.Equal(expectedLine, createdLine);
        }

        [Fact]
        public void CanCreateIndexOnAllIngredientsProperties()
        {
            ConstructQueryAccessibleProperties propertyPaths;
            var ingredientsQd = PrepareIngredientQueryForIndexCreation(out propertyPaths);

            var propertiesToIndex = TestQueries.IngredientsAllPropertiesToIndex();

            var indexDefiniton = _indexDefinitionCreater.CreateIndexDefinitionFor(ingredientsQd, propertyPaths, propertiesToIndex);
            const string expectedMap = "from doc in docs\r\nwhere doc[\"@metadata\"][\"Raven-Entity-Name\"] == \"Query/1\"\r\nselect new { \r\n_id = doc._id,\r\ntitle = ((IEnumerable<dynamic>)doc.title).DefaultIfEmpty().Select(x0 => \r\nx0._value),\r\ndescription = ((IEnumerable<dynamic>)doc.description).DefaultIfEmpty().Select(x0 => \r\nx0._value),\r\nhasPharmacologicalAction__id = ((IEnumerable<dynamic>)doc.hasPharmacologicalAction).DefaultIfEmpty().Select(x0 => \r\nx0._id),\r\nhasPharmacologicalAction_title = ((IEnumerable<dynamic>)doc.hasPharmacologicalAction).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.title).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nhasPharmacologicalAction_description = ((IEnumerable<dynamic>)doc.hasPharmacologicalAction).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.description).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nhasMechanismOfAction__id = ((IEnumerable<dynamic>)doc.hasMechanismOfAction).DefaultIfEmpty().Select(x0 => \r\nx0._id),\r\nhasMechanismOfAction_title = ((IEnumerable<dynamic>)doc.hasMechanismOfAction).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.title).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nhasPhysiologicEffect__id = ((IEnumerable<dynamic>)doc.hasPhysiologicEffect).DefaultIfEmpty().Select(x0 => \r\nx0._id),\r\nhasPhysiologicEffect_title = ((IEnumerable<dynamic>)doc.hasPhysiologicEffect).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.title).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nhasPharmacokinetics__id = ((IEnumerable<dynamic>)doc.hasPharmacokinetics).DefaultIfEmpty().Select(x0 => \r\nx0._id),\r\nhasPharmacokinetics_title = ((IEnumerable<dynamic>)doc.hasPharmacokinetics).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.title).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nhasPregnancyCategory = ((IEnumerable<dynamic>)doc.hasPregnancyCategory).DefaultIfEmpty().Select(x0 => \r\nx0),\r\nmayTreat__id = ((IEnumerable<dynamic>)doc.mayTreat).DefaultIfEmpty().Select(x0 => \r\nx0._id),\r\nmayTreat_title = ((IEnumerable<dynamic>)doc.mayTreat).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.title).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nmayTreat_description = ((IEnumerable<dynamic>)doc.mayTreat).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.description).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nmayPrevent__id = ((IEnumerable<dynamic>)doc.mayPrevent).DefaultIfEmpty().Select(x0 => \r\nx0._id),\r\nmayPrevent_title = ((IEnumerable<dynamic>)doc.mayPrevent).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.title).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nmayPrevent_description = ((IEnumerable<dynamic>)doc.mayPrevent).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.description).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\ncontraindicatedWith__id = ((IEnumerable<dynamic>)doc.contraindicatedWith).DefaultIfEmpty().Select(x0 => \r\nx0._id),\r\ncontraindicatedWith_title = ((IEnumerable<dynamic>)doc.contraindicatedWith).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.title).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\ncontraindicatedWith_description = ((IEnumerable<dynamic>)doc.contraindicatedWith).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.description).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nhasMedicinalProduct__id = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => \r\nx0._id),\r\nhasMedicinalProduct_title = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.title).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nhasMedicinalProduct_description = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => \r\nx0.description),\r\nhasMedicinalProduct_hasRouteOfAdministration = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => \r\nx0.hasRouteOfAdministration),\r\nhasMedicinalProduct_hasDosageForm = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => \r\nx0.hasDosageForm),\r\nhasMedicinalProduct_hasATCConcept__id = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.hasATCConcept).DefaultIfEmpty().Select(x1 => \r\nx1._id)),\r\nhasMedicinalProduct_hasATCConcept_prefLabel = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.hasATCConcept).DefaultIfEmpty().Select(x1 => \r\n((IEnumerable<dynamic>)x1.prefLabel).DefaultIfEmpty().Select(x2 => \r\nx2._value))),\r\nhasMedicinalProduct_hasATCConcept_notation = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.hasATCConcept).DefaultIfEmpty().Select(x1 => \r\n((IEnumerable<dynamic>)x1.notation).DefaultIfEmpty().Select(x2 => \r\nx2))),\r\n_metadata_Raven_Entity_Name = doc[\"@metadata\"][\"Raven-Entity-Name\"]}";

            Assert.Equal(expectedMap, indexDefiniton.RavenMap);
            const string expectedName = "Query/1/_id_title_description_hasPharmacologicalAction__id_hasPharmacologicalAction_title_hasPharmacologicalAction_description_hasMechanismOfAction__id_hasMechanismOfAction_title_hasPhysiologicEffect__id_hasPhysiologicEffect_title_hasPharmacokinetics__id_hasPharmacokinetics_title_hasPregnancyCategory_mayTreat__id_mayTreat_title_mayTreat_description_mayPrevent__id_mayPrevent_title_mayPrevent_description_contraindicatedWith__id_contraindicatedWith_title_contraindicatedWith_description_hasMedicinalProduct__id_hasMedicinalProduct_title_hasMedicinalProduct_description_hasMedicinalProduct_hasRouteOfAdministration_hasMedicinalProduct_hasDosageForm_hasMedicinalProduct_hasATCConcept__id_hasMedicinalProduct_hasATCConcept_prefLabel_hasMedicinalProduct_hasATCConcept_notation";
            Assert.Equal(expectedName, indexDefiniton.Name);
        }
    }
}

