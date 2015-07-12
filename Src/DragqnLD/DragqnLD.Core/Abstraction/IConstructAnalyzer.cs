using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.Query;
using Raven.Json.Linq;

namespace DragqnLD.Core.Abstraction
{
    public interface IConstructAnalyzer
    {
        RavenJObject CreateCompactionContextForQuery(QueryDefinition queryDefinition);
    }
}
