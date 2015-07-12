using System.IO;
using DragqnLD.Core.Implementations;
using JsonLD.Core;
using Raven.Json.Linq;

namespace DragqnLD.Core.Abstraction
{
    public interface IDataFormatter
    {
        void Format(TextReader input, TextWriter output, string rootObjectId, Context compactionContext, out PropertyMappings mappings);
    }
}
