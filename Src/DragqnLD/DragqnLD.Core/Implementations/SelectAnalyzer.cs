using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using VDS.RDF.Parsing;

namespace DragqnLD.Core.Implementations
{
    public class SelectAnalyzer : ISelectAnalyzer
    {
        public string ConvertSparqlToLuceneNoIndex(string sparql, ConstructQueryAccessibleProperties hierarchy)
        {
            var parser = new SparqlQueryParser();
            var parsedSparql = parser.ParseFromString(sparql);

            //find all property paths that are getting queried
            //  enumerate triples start from variable and traverse down all possible paths
            //  end of a path is designated by a literal, or by finding the variable in the FILTER clause

            //consult hierarchy for abbrevieted names and array wrapping 
            //  to construct correct lucene title name for each identified path, dont forget possible "_value" at the end
            //if path points to a objectProp -> add _id
            if (parsedSparql.RootGraphPattern.HasChildGraphPatterns)
                throw  new NotSupportedException("Child graph patterns are not supported");

            foreach (var triplePattern in parsedSparql.RootGraphPattern.TriplePatterns)
            {
                
            }
            
            

            throw new NotImplementedException();
        }
    }
}
