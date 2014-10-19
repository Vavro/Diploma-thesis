using System.IO;
using DragqnLD.Core.Implementations;

namespace DragqnLD.Core.Abstraction
{
    interface IDataFormatter
    {
        void Format(TextReader input, TextWriter output, string rootObjectId, out PropertyMappings mappings);
    }
}
