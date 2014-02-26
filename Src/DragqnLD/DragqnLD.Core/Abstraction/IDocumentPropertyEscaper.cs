using Newtonsoft.Json.Linq;

namespace DragqnLD.Core.Abstraction
{
    //todo: should be parametrized by settings from construct query analysis
    //query analysis should give out the replacement strings
    public interface IDocumentPropertyEscaper
    {
        void EscapeDocumentProperies(JObject document);
        string EscapePropertyPath(string propertyPath);
    }
}