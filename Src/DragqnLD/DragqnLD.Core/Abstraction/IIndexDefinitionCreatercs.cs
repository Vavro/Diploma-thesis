using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using DragqnLD.Core.Abstraction.Query;
using Raven.Abstractions.Indexing;

namespace DragqnLD.Core.Abstraction
{
    public class DragqnLDIndexRequirements
    {
        public readonly List<PropertyToIndex> PropertiesToIndex = new List<PropertyToIndex>();
    }

    public class DragqnLDIndexDefiniton
    {
        public string Name { get; set; }
        public DragqnLDIndexRequirements Requirements { get; set; }

        public string RavenMap { get; set; }
        public IDictionary<string, string> RavenAnalyzers { get; set; }
        public Dictionary<string, string> PropertyNameMap { get; set; }
    }

    public class PropertyToIndex
    {
        public string AbbreviatedName { get; set; }
        public bool Fulltext { get; set; }
    }

    interface IIndexDefinitionCreater
    {
        DragqnLDIndexDefiniton CreateIndexDefinitionFor(QueryDefinition ingredientsQd, ConstructQueryAccessibleProperties propertyPaths, DragqnLDIndexRequirements requirements);
    }
}
