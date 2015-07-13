using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
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
            var ingredientsQd = TestQueries.TestQueryDefinitionIngredients;

            var constructQueryAnalyzer = new ConstructAnalyzer();
            var parsedQuery = ConstructAnalyzerHelper.ReplaceParamAndParseConstructQuery(ingredientsQd);
            var compactionContext = constructQueryAnalyzer.CreateCompactionContextForQuery(parsedQuery);
            var propertyPaths = constructQueryAnalyzer.CreatePropertyPathsForQuery(parsedQuery, compactionContext);
            
            //add array info to property paths
            var inputDirectoryInfo = new DirectoryInfo(TestDataConstants.IngredientsFolder);
            var inputFiles = inputDirectoryInfo.GetFiles();
            var firstFile = inputFiles.First(file => file.Name == "M0000115.json");
            const string id = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115";
            var writer = new StringWriter();
            var convertedContext = DataLoaderHelper.ConvertCompactionContext(compactionContext);
            var output = ExpandedJsonLDDataFormatterTests.GetFormatted(firstFile.FullName, id, writer, convertedContext, propertyPaths);
            

            var propertiesToIndex = new DragqnLDIndexDefinition();
            propertiesToIndex.PropertiesToIndex.Add(new PropertiesToIndex() { AbbreviatedName = "@id"});

            var indexDefiniton = _indexDefinitionCreater.CreateIndexDefinitionFor(ingredientsQd, propertyPaths, propertiesToIndex);
            const string expectedMap = @"from doc in docs
where doc[""@metadata""][""Raven-Entity-Name""] == ""Query/1""
select new { 
_id = doc._id,
_metadata_Raven_Entity_Name = doc[""@metadata""][""Raven-Entity-Name""]}";

            Assert.Equal(expectedMap, indexDefiniton.Map);

        }

        [Fact]
        public void CanProduceRightFieldString()
        {
            var ingredientsQd = TestQueries.TestQueryDefinitionIngredients;

            var constructQueryAnalyzer = new ConstructAnalyzer();
            var parsedQuery = ConstructAnalyzerHelper.ReplaceParamAndParseConstructQuery(ingredientsQd);
            var compactionContext = constructQueryAnalyzer.CreateCompactionContextForQuery(parsedQuery);
            var propertyPaths = constructQueryAnalyzer.CreatePropertyPathsForQuery(parsedQuery, compactionContext);

            //add array info to property paths
            var inputDirectoryInfo = new DirectoryInfo(TestDataConstants.IngredientsFolder);
            var inputFiles = inputDirectoryInfo.GetFiles();
            var firstFile = inputFiles.First(file => file.Name == "M0000115.json");
            const string id = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115";
            var writer = new StringWriter();
            var convertedContext = DataLoaderHelper.ConvertCompactionContext(compactionContext);
            var output = ExpandedJsonLDDataFormatterTests.GetFormatted(firstFile.FullName, id, writer, convertedContext, propertyPaths);

            var tuple = _indexDefinitionCreater.CreateIndexedFieldNameAndAccess(propertyPaths,
                new PropertiesToIndex() {AbbreviatedName = "hasMedicinalProduct.hasATCConcept.prefLabel"});
            var createdLine = tuple.Item1 + " = " + tuple.Item2;

            const string expectedLine = @"hasMedicinalProduct_hasATCConcept_prefLabel = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => 
((IEnumerable<dynamic>)x0.hasATCConcept).DefaultIfEmpty().Select(x1 => 
((IEnumerable<dynamic>)x1.prefLabel).DefaultIfEmpty().Select(x2 => 
x2)))";

            Assert.Equal(expectedLine, createdLine);
        }
    }
}
