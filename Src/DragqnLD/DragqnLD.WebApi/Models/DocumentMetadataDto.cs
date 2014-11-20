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
}