using System;

namespace DragqnLD.Core.Abstraction.Query
{
    public class SparqlQueryInfo
    {
        public string Query { get; set; }
        public Uri SparqlEndpoint { get; set; }
        public Uri DefaultDataSet { get; set; }
    }
}