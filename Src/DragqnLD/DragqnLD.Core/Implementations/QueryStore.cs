using System;
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
                await session.StoreAsync(definition).ConfigureAwait(false);
                await session.SaveChangesAsync().ConfigureAwait(false);
                return definition.Id;
            }
        }

        public async Task<QueryDefinition> Get(string key)
        {
            using (var session = _store.OpenAsyncSession())
            {
                var queryDefinition = await session.LoadAsync<QueryDefinition>(key).ConfigureAwait(false);
                return queryDefinition;
            }
        }

        public async Task<IEnumerable<QueryDefinition>> GetAllDefinitions()
        {
            using (var session = _store.OpenAsyncSession())
            {
                return await session.Query<QueryDefinition>().ToListAsync().ConfigureAwait(false);
            }
        }

        public async Task UpdateLastRun(string definitionId, DateTime dateTime)
        {
            using (var session = _store.OpenAsyncSession())
            {
                var qd = await session.LoadAsync<QueryDefinition>(definitionId);
                qd.LastProcessed = dateTime;
                await session.SaveChangesAsync();
            }
        }
    }
}
