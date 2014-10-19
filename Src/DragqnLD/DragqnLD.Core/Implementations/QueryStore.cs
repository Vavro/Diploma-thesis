using System.Collections.Generic;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;
using Raven.Client;

namespace DragqnLD.Core.Implementations
{
    public class QueryStore : IQueryStore
    {
        //todo: Inject session from current call - Raven session will be tied to one REST call?

        private readonly IDocumentStore _store;
        
        public QueryStore(IDocumentStore store)
        {
            _store = store;
        }

        public async Task<string> Add(QueryDefinition definition)
        {
            using (var session = _store.OpenAsyncSession())
            {
                await session.StoreAsync(definition);
                await session.SaveChangesAsync();
                return definition.Id;
            }
        }

        public async Task<QueryDefinition> Get(string key)
        {
            using (var session = _store.OpenAsyncSession())
            {
                var queryDefinition = await session.LoadAsync<QueryDefinition>(key);
                return queryDefinition;
            }
        }

        public async Task<IEnumerable<QueryDefinition>> GetAllDefinitions()
        {
            using (var session = _store.OpenAsyncSession())
            {
                return await session.Query<QueryDefinition>().ToListAsync();
            }
        }
    }
}
