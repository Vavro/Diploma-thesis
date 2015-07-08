﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Indexes;
using JsonLD.Core;
using Microsoft.Win32;
using Raven.Client;
using Raven.Json.Linq;

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

        public async Task<int> GetDocumentCount(string definitionId)
        {
            using (var session = _store.OpenAsyncSession())
            {
                var countResult = await session.Query<Documents_CountByCollection.ReduceResult, Documents_CountByCollection>()
                    .Where(x => x.Name == definitionId).ToListAsync();
                var count = countResult.SingleOrDefault();
                if (count != null)
                {
                    return count.Count;
                }

                return 0;
            }
        }

        public async Task StoreMappings(string definitionId, PropertyMappings mappings)
        {
            using (var session = _store.OpenAsyncSession())
            {
                var qd = await session.LoadAsync<QueryDefinition>(definitionId);
                
                qd.Mappings = mappings.AsList();
                
                await session.SaveChangesAsync();
            }
        }

        public async Task<string> StoreContext(string definitionId, RavenJObject compactionContext)
        {
            //ravendb removes @properties - top one is @context, so remove the nesting and ad it in getContext
            var contextToStore = compactionContext.First().Value;
            var contextId = definitionId + "/Context";

            using (var session = _store.OpenAsyncSession())
            {
                await session.StoreAsync(contextToStore, contextId);

                var metadata = session.Advanced.GetMetadataFor(contextToStore);
                metadata["Raven-Entity-Name"] = "Contexts";
                await session.SaveChangesAsync().ConfigureAwait(false);

                await session.SaveChangesAsync();
            }

            return contextId;
        }

        public async Task<RavenJObject> GetContext(string definitionId)
        {
            var contextId = definitionId + "/Context";
            using (var session = _store.OpenAsyncSession())
            {
                var contextContent = await session.LoadAsync<RavenJObject>(contextId).ConfigureAwait(false);
                var context = new RavenJObject();
                context.Add("@context", contextContent);
                return context;
            }
        }
    }
}
