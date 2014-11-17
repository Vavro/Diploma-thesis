using System;
using System.Threading;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.Query;

namespace DragqnLD.Core.Abstraction
{
    /// <summary>
    /// The data loading inteface
    /// </summary>
    public interface IDataLoader
    {
        /// <summary>
        /// Loads the specified definition for indexing.
        /// </summary>
        /// <param name="definitionId"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="definition">The definition of queries to be indexed.</param>
        Task Load(string definitionId, CancellationToken cancellationToken, IProgress<QueryDefinitionStatus> progress);
    }
}
