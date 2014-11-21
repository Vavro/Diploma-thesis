using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DragqnLD.WebApi.Annotations;

namespace DragqnLD.WebApi.Models
{
    /// <summary>
    /// Stored document metadata
    /// </summary>
    [UsedImplicitly]
    public class DocumentMetadataDto
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [UsedImplicitly]
        public string Id { get; set; }
    }

    /// <summary>
    /// Result of a paged request for documents
    /// </summary>
    public class PagedDocumentMetadataDto
    {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IList<DocumentMetadataDto> Items { get; set; }

        /// <summary>
        /// Gets or sets the total documents.
        /// </summary>
        /// <value>
        /// The total documents.
        /// </value>
        public int TotalDocuments { get; set; }
    }
}