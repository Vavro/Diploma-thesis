using DragqnLD.Core.Interfaces.Query;

namespace DragqnLD.Core.Implementations
{
    public class QueryKey : IQueryKey
    {
        public QueryKey(string key)
        {
            Key = key;
        }

        public string Key { get; set; }
    }
}