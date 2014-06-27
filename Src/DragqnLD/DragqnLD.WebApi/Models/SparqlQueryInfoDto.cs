using System;

namespace DragqnLD.WebApi.Models
{
    public class SparqlQueryInfoDto
    {
        public string Query { get; set; }
        public Uri SparqlEndpoint { get; set; }
        public Uri DefaultDataSet { get; set; }
    }
}