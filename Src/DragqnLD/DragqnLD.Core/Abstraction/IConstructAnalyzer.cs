using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.Query;

namespace DragqnLD.Core.Abstraction
{
    public interface IConstructAnalyzer
    {
        JsonLD.Core.Context CreateCompactionContextForQuery(QueryDefinition queryDefinition);
    }
}
