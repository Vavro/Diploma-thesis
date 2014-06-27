using System.ComponentModel.DataAnnotations;

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
        public SparqlQueryInfoDto ConstructQuery { get; set; }

        /// <summary>
        /// Gets the name of the construct query parameter.
        /// </summary>
        /// <value>
        /// The name of the construct query parameter.
        /// </value>
        [Required]
        public string ConstructQueryUriParameterName { get; set; }

        /// <summary>
        /// Gets the select query.
        /// </summary>
        /// <value>
        /// The select query.
        /// </value>
        [Required]
        public SparqlQueryInfoDto SelectQuery { get; set; }
    }
}