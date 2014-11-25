using System;
using System.Collections.Generic;
using DragqnLD.Core.Implementations;

namespace DragqnLD.Core.Abstraction.Query
{
    /// <summary>
    /// Defines the a query that can be indexed by the system
    /// </summary>
    public class QueryDefinition
    {
        public QueryDefinition()
        {
            Mappings = new List<PropertyEscape>();
        }

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

        /// <summary>
        /// Gets or sets the last processed datetime.
        /// </summary>
        public DateTime? LastProcessed { get; set; }

        /// <summary>
        /// Gets or sets the mappings.
        /// </summary>
        /// <value>
        /// The mappings.
        /// </value>
        public List<PropertyEscape> Mappings { get; set; }
    }

    public static class QueryDefinitionExtensions
    {
        public static QueryDefinitionStatus GetStatus(this QueryDefinition qd)
        {
            return QueryDefinitionStatus.From(qd.LastProcessed == null ? QueryStatus.ReadyToRun : QueryStatus.Loaded);
        }
    }
}