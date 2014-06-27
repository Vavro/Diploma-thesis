using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DragqnLD.WebApi.Models
{

    public class QueryDefinitionMetadataDto
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }        
    }

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
        public SparqlQueryInfoDto ConstructQuery { get; set; }

        /// <summary>
        /// Gets the name of the construct query parameter.
        /// </summary>
        /// <value>
        /// The name of the construct query parameter.
        /// </value>
        public string ConstructQueryUriParameterName { get; set; }

        /// <summary>
        /// Gets the select query.
        /// </summary>
        /// <value>
        /// The select query.
        /// </value>
        public SparqlQueryInfoDto SelectQuery { get; set; }
    }

    public class SparqlQueryInfoDto
    {
        public string Query { get; set; }
        public Uri SparqlEndpoint { get; set; }
        public Uri DefaultDataSet { get; set; }
    }
}