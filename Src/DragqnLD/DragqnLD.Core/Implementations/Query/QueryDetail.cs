using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Interfaces.Query;

namespace DragqnLD.Core.Implementations
{
    public class QueryDetail : IQueryDetail
    {
        public QueryDetail(IQueryKey key, IQueryDefinition definition)
        {
            Key = key;
            Definition = definition;
        }

        public IQueryKey Key { get; set; }
        public IQueryDefinition Definition { get; set; }
    }
}
