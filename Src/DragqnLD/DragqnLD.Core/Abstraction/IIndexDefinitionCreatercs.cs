using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using DragqnLD.Core.Abstraction.Indexes;
using DragqnLD.Core.Abstraction.Query;
using Raven.Abstractions.Indexing;

namespace DragqnLD.Core.Abstraction
{
    interface IIndexDefinitionCreater
    {
        DragqnLDIndexDefiniton CreateIndexDefinitionFor(QueryDefinition queryDefinition, ConstructQueryAccessibleProperties propertyPaths, DragqnLDIndexRequirements requirements);
    }
}
