using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;

namespace DragqnLD.Core.Implementations
{
    class DataLoader : IDataLoader
    {
        private readonly IQueryStore _queryStore;

        public DataLoader(IQueryStore queryStore)
        {
            _queryStore = queryStore;
        }

        public async Task<string> Load(QueryDefinition definition)
        {
            //todo: query sparql endpoint for data and process results
            return await _queryStore.Add(definition);
        }
    }
}
