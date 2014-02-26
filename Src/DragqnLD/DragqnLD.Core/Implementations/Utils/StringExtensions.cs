using System.Collections.Generic;
using System.Text;

namespace DragqnLD.Core.Implementations.Utils
{
    public static class StringExtensions
    {
        public static bool ReplaceChars(this string s, HashSet<char> oldCharacters, char newCharacter, out string replacedString)
        {
            var replacedAnything = false;
            var sb = new StringBuilder(s);

            for (int index = 0; index < sb.Length; index++)
            {
                char c = sb[index];

                if (oldCharacters.Contains(c))
                {
                    sb[index] = newCharacter;
                    replacedAnything = true;
                }
            }

            replacedString = sb.ToString();
            return replacedAnything;
        }
    }
}