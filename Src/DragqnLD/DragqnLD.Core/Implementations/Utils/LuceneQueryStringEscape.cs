using DragqnLD.Core.Abstraction.Data;

namespace DragqnLD.Core.Implementations.Utils
{
    public static class LuceneQueryStringEscape
    {
        public static string EscapePropertyName(this string propertyName)
        {
            string output;
            propertyName.ReplaceChars(SpecialCharacters.ProblematicCharacterSet, SpecialCharacters.EscapeChar,
                out output);
            return output;
        }

        public static PropertyCondition AsCondition(this string propertyName, string propertyValue)
        {
            return new PropertyCondition() { PropertyName = propertyName, Value = propertyValue };
        }
    }
}