using System;

namespace DragqnLD.Core.Abstraction.Query
{
    /// <summary>
    /// Defines the a query that can be indexed by the system
    /// </summary>
    public class QueryDefinition
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

        /// <summary>
        /// Gets the construct query.
        /// </summary>
        /// <value>
        /// The construct query.
        /// </value>
        public SparqlQueryInfo ConstructQuery { get; set; }

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
        public SparqlQueryInfo SelectQuery { get; set; }
    }

    public class SparqlQueryInfo
    {
        public string Query { get; set; }
        public Uri SparqlEndpoint { get; set; }
        public Uri DefaultDataSet { get; set; }
    }
}