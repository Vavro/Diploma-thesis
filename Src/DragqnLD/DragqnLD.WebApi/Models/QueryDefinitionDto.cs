using System.ComponentModel.DataAnnotations;
using DragqnLD.WebApi.Annotations;

namespace DragqnLD.WebApi.Models
{
    /// <summary>
    /// Defines the a query that can be indexed by the system
    /// </summary>
    public class QueryDefinitionDto : QueryDefinitionMetadataDto
    {
        
        /// <summary>
        /// Gets the construct query.
        /// </summary>
        /// <value>
        /// The construct query.
        /// </value>
        [Required]
        [UsedImplicitly]
        public SparqlQueryInfoDto ConstructQuery { get; set; }

        /// <summary>
        /// Gets the name of the construct query parameter.
        /// </summary>
        /// <value>
        /// The name of the construct query parameter.
        /// </value>
        [Required]
        [UsedImplicitly]
        public string ConstructQueryUriParameterName { get; set; }

        /// <summary>
        /// Gets the select query.
        /// </summary>
        /// <value>
        /// The select query.
        /// </value>
        [Required]
        [UsedImplicitly]
        public SparqlQueryInfoDto SelectQuery { get; set; }
    }
}