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
            var sparqlQueryParser = new SparqlQueryParser();

            var queryDefinition = new QueryDefinition()
            {
                Name = "test query",
                Description = "testing query",
                ConstructQuery = @"DESCRIBE <http://linked.opendata.cz/resource/ATC/M01AE01>",
                ConstructQueryUriParameterName = "test",
                SelectQuery = @"SELECT DISTINCT ?s WHERE { ?s ?p ?o }"
            };

            var id = await queryStore.Add(queryDefinition);

            var retrievedQueryDefinition = await queryStore.Get(id);

            Assert.Equal(queryDefinition.Name, retrievedQueryDefinition.Name);
        }
    }
}
