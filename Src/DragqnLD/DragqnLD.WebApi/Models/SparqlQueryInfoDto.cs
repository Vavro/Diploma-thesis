using System.ComponentModel.DataAnnotations;
using DragqnLD.WebApi.Annotations;
using DragqnLD.WebApi.Validation;

namespace DragqnLD.WebApi.Models
{
    /// <summary>
    /// Data transfer object for Sparql Query info
    /// </summary>
    [UsedImplicitly]
    public class SparqlQueryInfoDto
    {
        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        [Required]
        [UsedImplicitly]
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets the sparql endpoint.
        /// </summary>
        /// <value>
        /// The sparql endpoint.
        /// </value>
        [Required]
        [UrlEx]
        [UsedImplicitly]
        public string SparqlEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the default data set.
        /// </summary>
        /// <value>
        /// The default data set.
        /// </value>
        [Required]
        [UrlEx]
        [UsedImplicitly]
        public string DefaultDataSet { get; set; }
    }
}