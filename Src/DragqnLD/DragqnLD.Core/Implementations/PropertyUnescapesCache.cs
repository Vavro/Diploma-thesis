using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;

namespace DragqnLD.Core.Implementations
{
    public class PropertyUnescapesCache : IPropertyUnescapesCache
    {
        //todo: if there are a lot of query definitions this probably shouldnt always stay in memory
        ConcurrentDictionary<string, PropertyMapForUnescape> _cachedMapForUnescapes = new ConcurrentDictionary<string, PropertyMapForUnescape>();
        
        public async Task<PropertyMapForUnescape> GetMapForUnescape(string queryId, Func<Task<List<PropertyEscape>>> loadFunc)
        {
            PropertyMapForUnescape propertyMapForUnescape;
            if (_cachedMapForUnescapes.TryGetValue(queryId, out propertyMapForUnescape))
            {
                return propertyMapForUnescape;
            }

            var newPropertyMapForUnescape = new PropertyMapForUnescape(await loadFunc());
            if (_cachedMapForUnescapes.TryAdd(queryId, newPropertyMapForUnescape))
            {
                return newPropertyMapForUnescape;
            }

            //race condition between two queryIds
            throw new NotSupportedException("Processing the same definition concurrently is not supported");
        }
    }
}
