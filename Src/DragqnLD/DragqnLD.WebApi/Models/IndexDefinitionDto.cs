using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DragqnLD.WebApi.Models
{
    /// <summary>
    /// Data transfer object for indexes
    /// </summary>
    public class IndexDefinitionsDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexDefinitionsDto"/> class.
        /// </summary>
        public IndexDefinitionsDto()
        {
            Indexes = new List<IndexDefinitionMetadataDto>();
        }

        /// <summary>
        /// Gets or sets the indexes.
        /// </summary>
        /// <value>
        /// The indexes.
        /// </value>
        public List<IndexDefinitionMetadataDto> Indexes { get; set; }

        /// <summary>
        /// Gets or sets the definition identifier.
        /// </summary>
        /// <value>
        /// The definition identifier.
        /// </value>
        public string DefinitionId { get; set; }

    }

    /// <summary>
    /// Data transfer object for index definition metadata
    /// </summary>
    public class IndexDefinitionMetadataDto
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the indexed fields.
        /// </summary>
        /// <value>
        /// The indexed fields.
        /// </value>
        public List<string> IndexedFields { get; set; }
    }

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
            PropertyPaths = new List<PropertyToIndexDto>();
        }

        /// <summary>
        /// Gets or sets the property paths that will be part of the index.
        /// </summary>
        /// <value>
        /// The property paths.
        /// </value>
        public List<PropertyToIndexDto> PropertyPaths { get; set; }
    }

    /// <summary>
    /// Data transfer object for one property to index data
    /// </summary>
    public class PropertyToIndexDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyToIndexDto"/> class.
        /// </summary>
        public PropertyToIndexDto()
        {
            
        }

        /// <summary>
        /// Gets or sets the property path.
        /// </summary>
        /// <value>
        /// The property path.
        /// </value>
        public string PropertyPath { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether [fulltext searchable].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [fulltext searchable]; otherwise, <c>false</c>.
        /// </value>
        public bool FulltextSearchable { get; set; }
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


    /// <summary>
    /// Data transfer object for transporting indexable properties list
    /// </summary>
    public class IndexablePropertiesDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexablePropertiesDto"/> class.
        /// </summary>
        public IndexablePropertiesDto()
        {
            Properties = new List<string>();
        }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public IEnumerable<string> Properties { get; set; }
    }
}