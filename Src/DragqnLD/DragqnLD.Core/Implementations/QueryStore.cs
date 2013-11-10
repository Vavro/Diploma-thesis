using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;
using Raven.Client;
using VDS.RDF.Query.Algebra;

namespace DragqnLD.Core.Implementations
{
    public class QueryStore : IQueryStore
    {
        //todo: Inject session from current call - Raven session will be tied to one REST call?

        private readonly IDocumentStore Store;
        
        public QueryStore(IDocumentStore store)
        {
            Store = store;
        }

        public async Task<string> Add(QueryDefinition definition)
        {
            using (var session = Store.OpenAsyncSession())
            {
                await session.StoreAsync(definition);
                await session.SaveChangesAsync();
                return definition.Id;
            }
        }

        public async Task<QueryDefinition> Get(string key)
        {
            using (var session = Store.OpenAsyncSession())
            {
                var queryDefinition = await session.LoadAsync<QueryDefinition>(key);
                return queryDefinition;
            }
        }

        public async Task<IEnumerable<QueryDefinition>> GetAllDefinitions()
        {
            using (var session = Store.OpenAsyncSession())
            {
                return await session.Query<QueryDefinition>().ToListAsync();
            }
        }
    }
}
