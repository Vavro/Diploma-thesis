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
            var inputFiles = inputDirectoryInfo.GetFiles();
            var firstFile = inputFiles.First(file => file.Name == "M0000115.json");
            const string id = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115";
            var writer = new StringWriter();
            var convertedContext = DataLoaderHelper.ConvertCompactionContext(compactionContext);
            var output = ExpandedJsonLDDataFormatterTests.GetFormatted(firstFile.FullName, id, writer, convertedContext,
                propertyPaths);
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
    }
}
