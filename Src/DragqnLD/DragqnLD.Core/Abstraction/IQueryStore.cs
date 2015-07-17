using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using DragqnLD.Core.Abstraction.Indexes;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;
using JsonLD.Core;
using Newtonsoft.Json.Linq;
using Raven.Abstractions.Indexing;
using Raven.Json.Linq;

namespace DragqnLD.Core.Abstraction
{
    /// <summary>
    /// The query store, contains all indexed queries
    /// </summary>
    public interface IQueryStore
    {
        /// <summary>
        /// Adds the specified definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>The key for this query.</returns>
        Task<string> Add(QueryDefinition definition);

        /// <summary>
        /// Gets the query definition for the specified key.
        /// </summary>
        /// <param name="id">The key.</param>
        /// <returns>The query definition.</returns>
        Task<QueryDefinition> Get(string id);

        /// <summary>
        /// Gets all query definitions.
        /// </summary>
        /// <returns>An enumerable list of query definitions</returns>
        Task<IEnumerable<QueryDefinition>> GetAllDefinitions();

        /// <summary>
        /// Updates the last run.
        /// </summary>
        /// <param name="definitionId"></param>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        Task UpdateLastRun(string definitionId, DateTime dateTime);

        /// <summary>
        /// Gets the document count.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        /// <returns></returns>
        Task<int> GetDocumentCount(string definitionId);

        /// <summary>
        /// Stores the mappings.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        /// <param name="mappings">The mappings.</param>
        /// <returns></returns>
        Task StoreMappings(string definitionId, PropertyMappings mappings);

        /// <summary>
        /// Stores the context.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        /// <param name="compactionContext">The compaction context.</param>
        /// <returns></returns>
        Task<string> StoreContext(string definitionId, CompactionContext compactionContext);

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        /// <returns></returns>
        Task<RavenJObject> GetContext(string definitionId);

        /// <summary>
        /// Stores the hierarchy.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        /// <param name="hierarchy">The hierarchy.</param>
        /// <returns></returns>
        Task<string> StoreHierarchy(string definitionId, ConstructQueryAccessibleProperties hierarchy);

        /// <summary>
        /// Gets the hierarchy.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        /// <returns></returns>
        Task<ConstructQueryAccessibleProperties> GetHierarchy(string definitionId);

        /// <summary>
        /// Creates the index.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        /// <param name="indexDefinition">The index definition.</param>
        /// <returns></returns>
        Task<string> StoreIndex(string definitionId, DragqnLDIndexDefiniton indexDefinition);

        Task<DragqnLDIndexDefinitions> GetIndexes(string definitionId);
    }
}
