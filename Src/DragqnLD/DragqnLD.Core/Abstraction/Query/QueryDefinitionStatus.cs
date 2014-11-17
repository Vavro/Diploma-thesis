using System.Security.Cryptography;

namespace DragqnLD.Core.Abstraction.Query
{
    //todo: add info into db, that states when was last run
    public class QueryDefinitionStatus
    {
        public QueryStatus Status { get; set; }
        public Progress DocumentLoadProgress { get; set; }

        public static QueryDefinitionStatus From(QueryStatus status)
        {
            return new QueryDefinitionStatus() {Status = status, DocumentLoadProgress = new Progress()};
        }
    }
}