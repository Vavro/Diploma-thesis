using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Database.Server.Responders;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using Xunit;

namespace DragqnLD.Core.UnitTests
{
    public class QueryStoreTests
    {
        public QueryStoreTests()
        {
            var docStore = new EmbeddableDocumentStore()
            {
                RunInMemory = true,
            };

            docStore.Initialize();

            documentStore = docStore;
            queryStore = new QueryStore(documentStore);
        }

        private readonly EmbeddableDocumentStore documentStore;
        private readonly QueryStore queryStore;
        
        [Fact]
        public async Task CanStoreQuery()
        {
            var queryDefinition = new QueryDefinition()
            {
                Name = "test query",
                Description = "testing query",
                ConstructQuery = new SparqlQueryInfo()
                {
                    Query = @"DESCRIBE <http://linked.opendata.cz/resource/ATC/M01AE01>",
                    DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                    SparqlEnpoint = new Uri(@"http://linked.opendata.cz/sparql")
                }, 
                ConstructQueryUriParameterName = "test",
                SelectQuery = new SparqlQueryInfo()
                {
                    Query = @"SELECT DISTINCT ?s WHERE { ?s ?p ?o }",
                    DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                    SparqlEnpoint = new Uri(@"http://linked.opendata.cz/sparql")
                }
            };

            var id = await queryStore.Add(queryDefinition);
            Console.WriteLine(id);

            var retrievedQueryDefinition = await queryStore.Get(id);

            Assert.Equal(queryDefinition.Name, retrievedQueryDefinition.Name);
            Assert.Equal(queryDefinition.Description, retrievedQueryDefinition.Description);
            Assert.Equal(queryDefinition.ConstructQuery.Query, retrievedQueryDefinition.ConstructQuery.Query);
            Assert.Equal(queryDefinition.ConstructQuery.DefaultDataSet, retrievedQueryDefinition.ConstructQuery.DefaultDataSet);
            Assert.Equal(queryDefinition.ConstructQuery.SparqlEnpoint, retrievedQueryDefinition.ConstructQuery.SparqlEnpoint);
            Assert.Equal(queryDefinition.ConstructQueryUriParameterName, retrievedQueryDefinition.ConstructQueryUriParameterName);
            Assert.Equal(queryDefinition.SelectQuery.Query, retrievedQueryDefinition.SelectQuery.Query);
            Assert.Equal(queryDefinition.SelectQuery.DefaultDataSet, retrievedQueryDefinition.SelectQuery.DefaultDataSet);
            Assert.Equal(queryDefinition.SelectQuery.SparqlEnpoint, retrievedQueryDefinition.SelectQuery.SparqlEnpoint);
        }
    }
}
