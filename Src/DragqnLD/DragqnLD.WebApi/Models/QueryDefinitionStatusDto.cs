using DragqnLD.Core.Abstraction.Query;
using DragqnLD.WebApi.Annotations;

namespace DragqnLD.WebApi.Models
{
    /// <summary>
    /// Data transfer object for Query Definition Status
    /// </summary>
    [UsedImplicitly]
    public class QueryDefinitionStatusDto
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public QueryStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the document load progress.
        /// </summary>
        /// <value>
        /// The document load progress.
        /// </value>
        public ProgressDto DocumentLoadProgress { get; set; }
    }
}