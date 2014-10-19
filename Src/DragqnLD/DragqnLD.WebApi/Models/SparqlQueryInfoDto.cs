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
        public string SparqlEndpoint { get; set; }
        [Required]
        [Url]
        public string DefaultDataSet { get; set; }
    }
}