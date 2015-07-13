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
    public class DragqnLDIndexDefinition
    {
        public readonly List<PropertiesToIndex> PropertiesToIndex = new List<PropertiesToIndex>();
    }

    public class PropertiesToIndex
    {
        public string AbbreviatedName { get; set; }
        public bool Fulltext { get; set; }
    }

    interface IIndexDefinitionCreater
    {
        IndexDefinition CreateIndexDefinitionFor(QueryDefinition ingredientsQd, ConstructQueryAccessibleProperties propertyPaths, DragqnLDIndexDefinition propertiesToIndex);
    }
}
