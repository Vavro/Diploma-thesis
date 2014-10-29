using DragqnLD.WebApi.Annotations;

namespace DragqnLD.WebApi.Models
{
    [UsedImplicitly]
    public class QueryDefinitionStatusDto
    {
        public QueryStatus Status { get; set; }
        public ProgressDto DocumentLoadProgress { get; set; }
    }
}