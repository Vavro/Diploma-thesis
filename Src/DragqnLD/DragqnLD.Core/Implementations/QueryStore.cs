﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using DragqnLD.Core.Abstraction.Indexes;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Indexes;
using JsonLD.Core;
using Microsoft.Win32;
using Raven.Abstractions.Indexing;
using Raven.Client;
using Raven.Client.Document;
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

        public async Task<string> StoreContext(string definitionId, CompactionContext compactionContext)
        {
            //ravendb removes @properties - top one is @context, so remove the nesting and ad it in getContext
            var contextToStore = compactionContext.BuildContext.First().Value;
            var contextId = GetContextId(definitionId);

            using (var session = _store.OpenAsyncSession())
            {
                await session.StoreAsync(contextToStore, contextId);

                var metadata = session.Advanced.GetMetadataFor(contextToStore);
                metadata["Raven-Entity-Name"] = "Contexts";
                await session.SaveChangesAsync().ConfigureAwait(false);
            }

            return contextId;
        }

        public async Task<RavenJObject> GetContext(string definitionId)
        {
            var contextId = GetContextId(definitionId);
            using (var session = _store.OpenAsyncSession())
            {
                var contextContent = await session.LoadAsync<RavenJObject>(contextId).ConfigureAwait(false);
                var context = new RavenJObject();
                context.Add("@context", contextContent ?? new RavenJObject());
                return context;
            }
        }

        public async Task<string> StoreHierarchy(string definitionId, ConstructQueryAccessibleProperties hierarchy)
        {
            var hierarchyId = GetHierarchyId(definitionId);

            using (var session = _store.OpenAsyncSession())
            {
                await session.StoreAsync(hierarchy, hierarchyId);

                var metadata = session.Advanced.GetMetadataFor(hierarchy);
                metadata["Raven-Entity-Name"] = "Hierarchy";
                await session.SaveChangesAsync().ConfigureAwait(false);
            }

            return hierarchyId;
        }

        public async Task<ConstructQueryAccessibleProperties> GetHierarchy(string definitionId)
        {
            var hierarchyId = GetHierarchyId(definitionId);
            using (var session = _store.OpenAsyncSession())
            {
                var hierarchy = await session.LoadAsync<ConstructQueryAccessibleProperties>(hierarchyId).ConfigureAwait(false);
                var hierarchyRootAsObject = hierarchy.RootProperty as IndexableObjectProperty;
                if (hierarchyRootAsObject != null)
                {
                    hierarchyRootAsObject.InitializeDictionaries();
                }
                return hierarchy;
            }
        }

        public async Task<string> StoreIndex(string definitionId, DragqnLDIndexDefiniton indexDefinition)
        {
            var definitionIndexesId = GetIndexId(definitionId);
            var indexRavenName = GetIndexRavenName(definitionId, indexDefinition.Name);
            using (var session = _store.OpenAsyncSession())
            {
                var currentIndexes = await session.LoadAsync<DragqnLDIndexDefinitions>(definitionIndexesId);
                if (currentIndexes == null)
                {
                    currentIndexes = new DragqnLDIndexDefinitions() { DefinitionId = definitionId };
                }
                currentIndexes.Indexes.Remove(indexDefinition.Name);
                currentIndexes.Indexes.Add(indexDefinition.Name, indexDefinition);

                await session.StoreAsync(currentIndexes, definitionIndexesId);

                var ravenIndexDefinition = new IndexDefinition()
                {
                    Map = indexDefinition.RavenMap,
                    Analyzers = indexDefinition.RavenAnalyzers
                };

                await session.Advanced.DocumentStore.AsyncDatabaseCommands.PutIndexAsync(indexRavenName,
                    ravenIndexDefinition, true);

                await session.SaveChangesAsync();
            }
            return indexRavenName;
        }

        private string GetIndexRavenName(string definitionId, string indexName)
        {
            if (indexName.StartsWith(definitionId))
            {
                return indexName;
            }
            throw new ArgumentException("Index name has to start with the definiton id");
        }

        private string GetIndexId(string definitionId)
        {
            return definitionId + "/Indexes";
        }
        private static string GetContextId(string definitionId)
        {
            return definitionId + "/Context";
        }

        private static string GetHierarchyId(string definitionId)
        {
            return definitionId + "/Hierarchy";
        }

        public async Task<DragqnLDIndexDefiniton> GetIndex(string definitionId, string indexName)
        {
            var definitionIndexesId = GetIndexId(definitionId);
            using (var session = _store.OpenAsyncSession())
            {
                var currentIndexes = await session.LoadAsync<DragqnLDIndexDefinitions>(definitionIndexesId);
                currentIndexes = currentIndexes ?? new DragqnLDIndexDefinitions() {DefinitionId = definitionId};
                
                DragqnLDIndexDefiniton index;
                var succ = currentIndexes.Indexes.TryGetValue(indexName, out index);
                if (!succ)
                {
                    throw new ArgumentOutOfRangeException("indexName",
                        String.Format("index name {0} is not present in query definition {1}", indexName, definitionId));
                }
                return index;
            }
        }

        public async Task<DragqnLDIndexDefinitions> GetIndexes(string definitionId)
        {
            var definitionIndexesId = GetIndexId(definitionId);
            using (var session = _store.OpenAsyncSession())
            {
                var currentIndexes = await session.LoadAsync<DragqnLDIndexDefinitions>(definitionIndexesId);
                return currentIndexes ?? new DragqnLDIndexDefinitions() { DefinitionId = definitionId };
            }
        }
    }

}
