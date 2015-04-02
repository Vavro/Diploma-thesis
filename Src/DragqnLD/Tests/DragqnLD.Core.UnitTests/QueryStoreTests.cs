using System;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;
using Raven.Client.Embedded;
using Xunit;
using Xunit.Abstractions;

namespace DragqnLD.Core.UnitTests
{

    [Trait("Category", "Basic")]
    public class QueryStoreTests : TestsBase
    {
        public QueryStoreTests(ITestOutputHelper output) : base(output)
        {
            var docStore = new EmbeddableDocumentStore
            {
                RunInMemory = true,
            };

            docStore.Initialize();

            _documentStore = docStore;
            _queryStore = new QueryStore(_documentStore);
        }

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly EmbeddableDocumentStore _documentStore;
        private readonly QueryStore _queryStore;
        
        [Fact]
        public async Task CanStoreQuery()
        {
            var queryDefinition = new QueryDefinition
            {
                Name = "test query",
                Description = "testing query",
                ConstructQuery = new SparqlQueryInfo
                {
                    Query = @"DESCRIBE <http://linked.opendata.cz/resource/ATC/M01AE01>",
                    DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                    SparqlEndpoint = new Uri(@"http://linked.opendata.cz/sparql")
                }, 
                ConstructQueryUriParameterName = "test",
                SelectQuery = new SparqlQueryInfo
                {
                    Query = @"SELECT DISTINCT ?s WHERE { ?s ?p ?o }",
                    DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                    SparqlEndpoint = new Uri(@"http://linked.opendata.cz/sparql")
                }
            };

            var id = await _queryStore.Add(queryDefinition);
            Output.WriteLine(id);

            var retrievedQueryDefinition = await _queryStore.Get(id);

            Assert.Equal(queryDefinition.Name, retrievedQueryDefinition.Name);
            Assert.Equal(queryDefinition.Description, retrievedQueryDefinition.Description);
            Assert.Equal(queryDefinition.ConstructQuery.Query, retrievedQueryDefinition.ConstructQuery.Query);
            Assert.Equal(queryDefinition.ConstructQuery.DefaultDataSet, retrievedQueryDefinition.ConstructQuery.DefaultDataSet);
            Assert.Equal(queryDefinition.ConstructQuery.SparqlEndpoint, retrievedQueryDefinition.ConstructQuery.SparqlEndpoint);
            Assert.Equal(queryDefinition.ConstructQueryUriParameterName, retrievedQueryDefinition.ConstructQueryUriParameterName);
            Assert.Equal(queryDefinition.SelectQuery.Query, retrievedQueryDefinition.SelectQuery.Query);
            Assert.Equal(queryDefinition.SelectQuery.DefaultDataSet, retrievedQueryDefinition.SelectQuery.DefaultDataSet);
            Assert.Equal(queryDefinition.SelectQuery.SparqlEndpoint, retrievedQueryDefinition.SelectQuery.SparqlEndpoint);
        }
    }
}
