using System.ComponentModel.DataAnnotations;
using DragqnLD.WebApi.Annotations;

namespace DragqnLD.WebApi.Models
{
    [UsedImplicitly]
    public class SparqlQueryInfoDto
    {
        [Required]
        [UsedImplicitly]
        public string Query { get; set; }
        [Required]
        [Url]
        [UsedImplicitly]
        public string SparqlEndpoint { get; set; }
        [Required]
        [Url]
        [UsedImplicitly]
        public string DefaultDataSet { get; set; }
    }
}