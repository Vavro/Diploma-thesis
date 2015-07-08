using System;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;
using JsonLD.Core;
using Newtonsoft.Json.Linq;
using Raven.Client.Embedded;
using Raven.Database.Config;
using Raven.Json.Linq;
using Raven.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace DragqnLD.Core.UnitTests
{
    public class QueryStoreFixture : RavenTestBase, IDisposable
    {
        public readonly QueryStore QueryStore;
        public readonly EmbeddableDocumentStore DocumentStore;

        protected const int RavenWebUiPort = 8081;

        protected override void ModifyConfiguration(InMemoryRavenConfiguration configuration)
        {
            configuration.Storage.Voron.AllowOn32Bits = true;

            base.ModifyConfiguration(configuration);
        }

        public QueryStoreFixture()
        {
            //NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(RavenWebUiPort);
            var docStore = NewDocumentStore(port: RavenWebUiPort);

            DocumentStore = docStore;
            QueryStore = new QueryStore(docStore);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                DocumentStore.Dispose();

                GC.Collect(2);
                GC.WaitForPendingFinalizers();
            }
        }
    }


    [Trait("Category", "Basic")]
    public class QueryStoreTests : TestsBase, IClassFixture<QueryStoreFixture>
    {
        public QueryStoreTests(ITestOutputHelper output, QueryStoreFixture queryStoreFixture) : base(output)
        {
            _documentStore = queryStoreFixture.DocumentStore;
            _queryStore = queryStoreFixture.QueryStore;
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

        
        [Fact]
        public async Task CanStoreAndLoadContext()
        {
            var context = new RavenJObject();
            var contextContent = new RavenJObject();
            context.Add("@context", contextContent);

            contextContent.Add("enc", new RavenJValue("http://http://linked.opendata.cz/ontology/drug-encyclopedia/"));
            contextContent.Add("skos", new RavenJValue("http://www.w3.org/2004/02/skos/core"));

            const string definitionId = "Query/1";

            await _queryStore.StoreContext(definitionId, context);

            //RavenTestBase.WaitForUserToContinueTheTest(_documentStore);

            var loadedContext = await _queryStore.GetContext(definitionId);

            //RavenTestBase.WaitForUserToContinueTheTest(_documentStore);

            Assert.Equal(context.ToString(), loadedContext.ToString());
        }
    }
}
