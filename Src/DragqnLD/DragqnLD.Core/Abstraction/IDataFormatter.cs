using System.IO;
using DragqnLD.Core.Implementations;

namespace DragqnLD.Core.Abstraction
{
    public interface IDataFormatter
    {
        void Format(TextReader input, TextWriter output, string rootObjectId, out PropertyMappings mappings);
    }
}
