using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;
using JsonLD.Core;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace DragqnLD.Core.Implementations
{
    public class ConstructAnalyzer : IConstructAnalyzer
    {
        public Context CreateCompactionContextForQuery(QueryDefinition queryDefinition)
        {
            var constructQuery = queryDefinition.ConstructQuery.Query;
            var constructParameterName = queryDefinition.ConstructQueryUriParameterName;
            
            var parametrizedSparqlString = new SparqlParameterizedString(constructQuery);
            
            var parser = new SparqlQueryParser();
            var sparqlQuery =parser.ParseFromString(parametrizedSparqlString);
            
            
            return new Context();
        }
    }
}
