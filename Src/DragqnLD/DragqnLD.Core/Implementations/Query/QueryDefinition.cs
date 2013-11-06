using DragqnLD.Core.Interfaces.Query;
using VDS.RDF.Query;

namespace DragqnLD.Core.Implementations
{
    public class QueryDefinition : IQueryDefinition
    {
        public QueryDefinition(string name, string description, SparqlParameterizedString constructQuery, string constructQueryUriParameterName, SparqlQuery selectQuery)
        {
            Name = name;
            Description = description;
            ConstructQuery = constructQuery;
            ConstructQueryUriParameterName = constructQueryUriParameterName;
            SelectQuery = selectQuery;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public SparqlParameterizedString ConstructQuery { get; set; }
        public string ConstructQueryUriParameterName { get; set; }
        public SparqlQuery SelectQuery { get; set; }
    }
}