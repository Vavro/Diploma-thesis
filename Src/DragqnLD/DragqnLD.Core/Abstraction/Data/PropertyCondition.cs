using System;
using System.Collections.Generic;
using DragqnLD.Core.Implementations.Utils;

namespace DragqnLD.Core.Abstraction.Data
{
    public class PropertyCondition
    {
        public string PropertyPath { get; set; }
        public string Value { get; set; }
    }

    public class PropertyPath
    {
        //stores the format of the path
        //i.e. 
        //"http://linked.opendata.cz/ontology/drug-encyclopedia/mayTreat","http://linked.opendata.cz/ontology/drug-encyclopedia/title","@value"
        //where "," delimits array/list access
        //"." member access
        private string path = null;

        public IEnumerable<PropertyPathPart> GetParts()
        {
            string currentMemberName = null;
            MemberAccessType currentMemberAccessType = MemberAccessType.Single;
            bool currentMemberAccessTypeSet = false;

            for (int i = 0; i < path.Length; i++)
            {
                char c = path[i];
                switch (c)
                {
                    case '"':
                        if (currentMemberName != null)
                        {
                            if (!currentMemberAccessTypeSet && (i < (path.Length - 1)) ) 
                            {
                                throw new NotSupportedException(String.Format("In Path {0} no member access type specified between two member names, character index {1}", path, i));
                            }
                            yield return new PropertyPathPart() {MemberName = currentMemberName, Type = currentMemberAccessType};
                            currentMemberAccessType = MemberAccessType.Single;
                            currentMemberAccessTypeSet = false;
                        }
                        i++;
                        currentMemberName = ReadMemberNameFrom(ref i);
                        break;
                    case ',':
                        if (currentMemberAccessTypeSet)
                        {
                            throw new NotSupportedException(String.Format("The path {0} contains badly formatted member access on character index {1}", path, i));
                        }
                        currentMemberAccessType = MemberAccessType.List;
                        currentMemberAccessTypeSet = true;
                        break;
                    case '.':
                        if (currentMemberAccessTypeSet)
                        {
                            throw new NotSupportedException(String.Format("The path {0} contains badly formatted member access on character index {1}", path, i));
                        }
                        currentMemberAccessType = MemberAccessType.Single;
                        currentMemberAccessTypeSet = true;
                        break;
                    default:
                        throw new NotSupportedException(String.Format("The path {0} is badly formatted on character index {1}", path, i));
                }
            }

            
        }

        public string GetEscapedRavenName()
        {
            string output;
            path.ReplaceChars(SpecialCharacters.ProblematicCharacterSet, SpecialCharacters.EscapeChar,
                out output);
            return output;
        }

        private string ReadMemberNameFrom(ref int index)
        {
            for (int i = index; i < path.Length; i++)
            {
                if (path[i] == '"')
                {
                    i++;
                    string memberName = path.Substring(index, i - index);
                    index = i;
                    return memberName;
                }
            }
            throw new NotSupportedException(String.Format("Path {0} doesn't contain closing \" for starting \" on character {1}", path, index));
        }
    }

    public class PropertyPathPart
    {
        public string MemberName { get; set; }
        public MemberAccessType Type { get; set; }
    }

    public enum MemberAccessType
    {
        Single, //todo: better name like single?
        List
    }
}