using System.IO;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using DragqnLD.Core.Implementations;
using JsonLD.Core;

namespace DragqnLD.Core.Abstraction
{
    public interface IDataFormatter
    {
        void Format(TextReader input, TextWriter output, string rootObjectId, Context compactionContext, ConstructQueryAccessibleProperties accessibleProperties, out PropertyMappings mappings);
    }
}
