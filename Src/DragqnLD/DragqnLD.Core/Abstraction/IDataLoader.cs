using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.Query;

namespace DragqnLD.Core.Abstraction
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
        Task<string> Load(QueryDefinition definition);
    }
}
