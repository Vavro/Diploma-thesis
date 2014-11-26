using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;

namespace DragqnLD.Core.Abstraction
{
    public interface IPropertyUnescapesCache
    {
        Task<PropertyMapForUnescape> GetMapForUnescape(string queryId, Func<Task<List<PropertyEscape>>> loadFunc);
    }
}
