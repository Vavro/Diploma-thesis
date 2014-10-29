using DragqnLD.WebApi.Annotations;

namespace DragqnLD.WebApi.Models
{
    [UsedImplicitly]
    public class QueryDefinitionWithStatusDto : QueryDefinitionDto
    {
        public QueryDefinitionStatusDto Status { get; set; }
        public int StoredDocumentCount { get; set; }

    }
}