using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Interfaces.Query;

namespace DragqnLD.Core.Interfaces
{
    /// <summary>
    /// The data loading inteface
    /// </summary>
    interface IDataLoader
    {
        /// <summary>
        /// Loads the specified definition for indexing.
        /// </summary>
        /// <param name="definition">The definition of queries to be indexed.</param>
        Task<IQueryKey> Load(IQueryDefinition definition);
    }
}
