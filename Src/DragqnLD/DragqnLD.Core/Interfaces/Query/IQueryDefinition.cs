using VDS.RDF.Query;

namespace DragqnLD.Core.Interfaces.Query
{
    /// <summary>
    /// Defines the a query that can be indexed by the system
    /// </summary>
    interface IQueryDefinition
    {

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        string Description { get; }

        /// <summary>
        /// Gets the construct query.
        /// </summary>
        /// <value>
        /// The construct query.
        /// </value>
        SparqlParameterizedString ConstructQuery { get; }

        /// <summary>
        /// Gets the name of the construct query parameter.
        /// </summary>
        /// <value>
        /// The name of the construct query parameter.
        /// </value>
        string ConstructQueryUriParameterName { get; }

        /// <summary>
        /// Gets the select query.
        /// </summary>
        /// <value>
        /// The select query.
        /// </value>
        SparqlQuery SelectQuery { get; }
    }
}