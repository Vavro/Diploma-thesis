using System.ComponentModel.DataAnnotations;
using DragqnLD.WebApi.Annotations;
using DragqnLD.WebApi.Validation;

namespace DragqnLD.WebApi.Models
{
    [UsedImplicitly]
    public class SparqlQueryInfoDto
    {
        [Required]
        [UsedImplicitly]
        public string Query { get; set; }
        [Required]
        [UrlEx]
        [UsedImplicitly]
        public string SparqlEndpoint { get; set; }
        [Required]
        [UrlEx]
        [UsedImplicitly]
        public string DefaultDataSet { get; set; }
    }
}