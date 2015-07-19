using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using DragqnLD.Core.Abstraction.Indexes;

namespace DragqnLD.Core.Abstraction
{
    public interface ISelectAnalyzer
    {
        string ConvertSparqlToLuceneNoIndex(string sparql, ConstructQueryAccessibleProperties hierarchy);

        string ConvertSparqlToLuceneWithIndex(string sparql, ConstructQueryAccessibleProperties hierarchy,
            DragqnLDIndexDefiniton indexDefinition);
    }
}
