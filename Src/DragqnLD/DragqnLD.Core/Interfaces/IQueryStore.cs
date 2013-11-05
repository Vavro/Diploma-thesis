using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Interfaces.Query;

namespace DragqnLD.Core.Interfaces
{
    /// <summary>
    /// The query store, contains all indexed queries
    /// </summary>
    interface IQueryStore
    {
        /// <summary>
        /// Adds the specified definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>The key for this query.</returns>
        IQueryKey Add(IQueryDefinition definition);

        /// <summary>
        /// Gets the query definition for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The query definition.</returns>
        IQueryDefinition Get(IQueryKey key);

        /// <summary>
        /// Gets all query definitions.
        /// </summary>
        /// <returns>An enumerable list of query definitions</returns>
        IEnumerable<IQueryDetail> GetAllDefinitions();

    }
}
