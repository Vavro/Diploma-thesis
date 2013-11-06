using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Interfaces;
using DragqnLD.Core.Interfaces.Query;
using Raven.Client;
using VDS.RDF.Query.Algebra;

namespace DragqnLD.Core.Implementations
{
    class StoredQuery
    {
        public string Id { get; set; }
        public IQueryDefinition QueryDefinition { get; set; }
    }

    public class QueryStore : IQueryStore
    {
        //todo: Inject session from current call

        private readonly IDocumentStore Store;
        
        public QueryStore(IDocumentStore store)
        {
            Store = store;
        }

        public async Task<IQueryKey> Add(IQueryDefinition definition)
        {
            using (var session = Store.OpenAsyncSession())
            {
                var storedQuery = new StoredQuery() {QueryDefinition = definition};
                await session.StoreAsync(storedQuery);
                await session.SaveChangesAsync();
                return new QueryKey(storedQuery.Id);
            }
        }

        public async Task<IQueryDefinition> Get(IQueryKey key)
        {
            using (var session = Store.OpenAsyncSession())
            {
                var storedQuery = await session.LoadAsync<StoredQuery>(key.Key);
                return storedQuery.QueryDefinition;
            }
        }

        public async Task<IEnumerable<IQueryDetail>> GetAllDefinitions()
        {
            using (var session = Store.OpenAsyncSession())
            {
                var allQueries = await session.Query<StoredQuery>().ToListAsync();
                return 
                    allQueries.Select(
                        storedQuery => 
                            new QueryDetail(new QueryKey(storedQuery.Id), storedQuery.QueryDefinition));
            }
        }
    }
}
