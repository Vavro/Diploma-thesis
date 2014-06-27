using System;
using System.ComponentModel.DataAnnotations;

namespace DragqnLD.WebApi.Models
{
    public class SparqlQueryInfoDto
    {
        [Required]
        public string Query { get; set; }
        [Required]
        [Url]
        public Uri SparqlEndpoint { get; set; }
        [Required]
        [Url]
        public Uri DefaultDataSet { get; set; }
    }
}