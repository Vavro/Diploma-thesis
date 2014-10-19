using DragqnLD.Core.Abstraction.Data;

namespace DragqnLD.Core.Implementations.Utils
{
    public static class LuceneQueryStringExtensions
    {
        public static PropertyCondition AsCondition(this string propertyName, string propertyValue)
        {
            return new PropertyCondition { PropertyPath = propertyName, Value = propertyValue };
        }
    }
}