using DragqnLD.WebApi.Annotations;

namespace DragqnLD.WebApi.Models
{
    /// <summary>
    /// Data transfer object for Query Definition with Status
    /// </summary>
    [UsedImplicitly]
    public class QueryDefinitionWithStatusDto : QueryDefinitionDto
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public QueryDefinitionStatusDto Status { get; set; }

        /// <summary>
        /// Gets or sets the stored document count.
        /// </summary>
        /// <value>
        /// The stored document count.
        /// </value>
        public int StoredDocumentCount { get; set; }

    }
}