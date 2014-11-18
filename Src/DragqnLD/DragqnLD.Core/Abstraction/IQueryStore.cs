using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.Query;

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
    }
}
