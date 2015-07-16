using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DragqnLD.WebApi.Models
{
    /// <summary>
    /// Data transfer object for DragqnLD index definitions in RavenDB
    /// </summary>
    public class IndexDefinitionDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexDefinitionDto"/> class.
        /// </summary>
        public IndexDefinitionDto()
        {
            RavenAnalyzers = new Dictionary<string, string>();
            PropertyNameMap = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the raven map.
        /// </summary>
        /// <value>
        /// The raven map.
        /// </value>
        public string RavenMap { get; set; }

        /// <summary>
        /// Gets or sets the raven analyzers.
        /// </summary>
        /// <value>
        /// The raven analyzers.
        /// </value>
        public Dictionary<string, string> RavenAnalyzers { get; set; }

        /// <summary>
        /// Gets or sets the property name map.
        /// </summary>
        /// <value>
        /// The property name map.
        /// </value>
        public Dictionary<string, string> PropertyNameMap { get; set; }
    }

    /// <summary>
    /// Properties to index data transfer object for index creation
    /// </summary>
    public class PropertiesToIndexDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesToIndexDto"/> class.
        /// </summary>
        public PropertiesToIndexDto()
        {
            PropertyPaths = new List<string>();
        }

        /// <summary>
        /// Gets or sets the property paths that will be part of the index.
        /// </summary>
        /// <value>
        /// The property paths.
        /// </value>
        public List<string> PropertyPaths { get; set; }
    }

    /// <summary>
    /// Data transfer object for transporting sparql select queries
    /// </summary>
    public class SparqlDto
    {
        /// <summary>
        /// Gets or sets the sparql select query.
        /// </summary>
        /// <value>
        /// The sparql select query.
        /// </value>
        public string SparqlSelectQuery { get; set; }
    }
}