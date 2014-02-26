using System.Collections.Generic;

namespace DragqnLD.Core.Implementations.Utils
{
    public static class SpecialCharacters
    {
        public const char EscapeChar = '_';
        //todo: problem - . (dot) , (comma) are used in raven for accessing property, array - for now it works as there are always arrays and no link contains a comma
        //problem only in usage for escaping whole path in lucene queries
        public static HashSet<char> ProblematicCharacterSet = new HashSet<char>() {':', '/', '-', '#', '@', '.'};
    }
}