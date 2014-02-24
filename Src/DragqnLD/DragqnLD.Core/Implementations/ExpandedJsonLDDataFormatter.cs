using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using VDS.RDF.Query.Expressions.Functions.XPath.String;

namespace DragqnLD.Core.Implementations
{
    public class ExpandedJsonLDDataFormatter : IDataFormatter
    {
        private class FlatGraphNester
        {
            private ReadOnlyDictionary<string, JObject> _graphObjectsById;
            private readonly JEnumerable<JToken> _graphObjects;
            private readonly string _rootObjectId;
            private JObject _rootJObject;
            private Stack<string> _recursiveIds; 

            public JObject RootJObject { get { return _rootJObject; } }

            public FlatGraphNester(JEnumerable<JToken> graphObjects, string rootObjectId)
            {
                _graphObjects = graphObjects;
                _rootObjectId = rootObjectId;
                _rootJObject = null;
                _graphObjectsById = null;
                _recursiveIds = new Stack<string>();
            }

            public void NestEverythingIntoRootObject()
            {
                ReplaceReferencePropertiesInJObject(_rootJObject);
            }
            
            private void ReplaceReferencePropertiesInJObject(JObject obj)
            {
                //check recursive path
                var objId = (string)obj["@id"];
                if (objId != null)
                {
                    if (_recursiveIds.Contains(objId))
                    {
                        //todo: add own exception
                        throw new NotSupportedException(String.Format("item id: {0} creates a recursion, whole previous id path: {1}", objId, String.Join(" ,", _recursiveIds.Reverse())));
                    }

                    _recursiveIds.Push(objId);
                }

                foreach (var property in obj.Properties())
                {
                    if (property.Name.Length > 1
                        && property.Name[0] == '@') //skip all JSON-LD keyword properties - @id, @type, @context etc.
                    {
                        continue;
                    }
                    
                    // go throught nested object and replace ids
                    JObject asJObject;
                    JValue asJValue;
                    JArray asJArray;
                    if ((asJObject = property.Value as JObject) != null)
                    {
                        ReplaceReferencePropertiesInJObject(asJObject);
                    }
                    //go throught and detect whether it contains more objects, or ids
                    else if ((asJArray= property.Value as JArray) != null)
                    {
                        for (int i = 0; i < asJArray.Count; i++)
                        {
                            var value = asJArray[i];
                            JValue valueAsJValue;
                            JObject valueAsJObject;
                            if ((valueAsJValue = value as JValue) != null)
                            {
                                var searchedId = valueAsJValue.ToString();
                                JObject objectFromGraph;
                                if (_graphObjectsById.TryGetValue(searchedId, out objectFromGraph))
                                {
                                    //todo: optimize this!!! - not needed to go through all objects again and again if they are present multiple times in the graph
                                    ReplaceReferencePropertiesInJObject(objectFromGraph);

                                    asJArray[i] = objectFromGraph;
                                }
                            }
                            else if ((valueAsJObject = value as JObject) != null)
                            {
                                ReplaceReferencePropertiesInJObject(valueAsJObject);
                            }
                        }
                    }
                    // replace if it is an id? - not presented in sample data
                    else if ((asJValue  = property.Value as JValue) != null)
                    {
                        //maybe detect whether value is an IRI
                        //lookup id in dictionary of objects, if id is found, replace
                        var searchedId = asJValue.ToString();
                        JObject objectFromGraph;
                        if (_graphObjectsById.TryGetValue(searchedId, out objectFromGraph))
                        {
                            //todo: optimize this!!! - not needed to go through all objects again and again if they are present multiple times in the graph
                            ReplaceReferencePropertiesInJObject(objectFromGraph);

                            property.Value = objectFromGraph;
                        }
                        else
                        {
                            //log not found?
                        }
                    }
                }

                if (objId != null)
                {
                    _recursiveIds.Pop();
                }
            }


            public void ReadObjectsFromGraph()
            {
                var _objects = new Dictionary<string, JObject>();
                foreach (var graphObject in _graphObjects)
                {
                    var id = (string)graphObject["@id"];
                    var typed = graphObject as JObject;
                    if (id == null || typed == null)
                        throw new NotSupportedException(
                            String.Format(
                                "The collection of objects in the graph property, didn't contain only objects with ids - invalid json: {0}",
                                graphObject.ToString()));

                    if (id == _rootObjectId)
                    {
                        _rootJObject = typed;
                    }
                    else
                    {
                        _objects.Add(id, typed);
                    }
                }

                if (_rootJObject == null)
                    throw new NotSupportedException(
                        String.Format("The collection of objects in the graph property, didn't contain the object with id: {0}",
                            _rootObjectId));

                _graphObjectsById = new ReadOnlyDictionary<string, JObject>(_objects);
            }
        }

        public void Format(TextReader input, TextWriter output, string rootObjectId, out PropertyMappings mappings)
        {
            //might be faster by using the strings via JsonTextReader, instead of Deserializing to JObject

            var inputString = input.ReadToEnd(); //TODO: to async

            var originalObject = JObject.Parse(inputString);
            
            var graphObjects = originalObject["@graph"].Children();

            var flatGraphNester = new FlatGraphNester(graphObjects, rootObjectId);

            flatGraphNester.ReadObjectsFromGraph();

            flatGraphNester.NestEverythingIntoRootObject();

            var rootJObject = flatGraphNester.RootJObject;

            //todo: escape property names before reformat (better perf), but will need to handle keywords (@type, @id, @context etc.) to not be reformatted
            var propertyEscaper = new DocumentPropertyEscaper();
            propertyEscaper.EscapeDocumentProperies(rootJObject);
            mappings = propertyEscaper.PropertyMappings;

            var o = rootJObject.ToString(Formatting.Indented);

            var jsonWriter = new JsonTextWriter(output);
            rootJObject.WriteTo(jsonWriter);
        }


    }

    //todo: should be parametrized by settings from construct query analysis
    internal interface IDocumentPropertyEscaper
    {
        void EscapeDocumentProperies(JObject document);
    }

    //todo: should be parametrized by settings from construct query analysis
    internal class DocumentPropertyEscaper : IDocumentPropertyEscaper
    {
        private PropertyMappings propertyMappings = new PropertyMappings();
        public PropertyMappings PropertyMappings { get { return propertyMappings; } }

        public void EscapeDocumentProperies(JObject document)
        {
            EscapePropertiesInJObject(document);
        }

        private void EscapePropertiesInJObject(JObject document)
        {
            var propertyNamesToReplace = new Dictionary<string, string>();

            foreach (var property in document.Properties())
            {
                string replacedPropertyName;
                var replacedAnything = property.Name.ReplaceChars(SpecialCharacters.ProblematicCharacterSet, SpecialCharacters.EscapeChar, out replacedPropertyName);
                if (replacedAnything)
                {
                    propertyNamesToReplace.Add(property.Name, replacedPropertyName);
                }

                JObject asJObject;
                JArray asJArray;
                if ((asJObject = property.Value as JObject) != null)
                {
                    EscapePropertiesInJObject(asJObject);
                }
                //go throught and detect whether it contains more objects
                else if ((asJArray = property.Value as JArray) != null)
                {
                    for (int i = 0; i < asJArray.Count; i++)
                    {
                        var value = asJArray[i];
                        JObject valueAsJObject;
                        if ((valueAsJObject = value as JObject) != null)
                        {
                            EscapePropertiesInJObject(valueAsJObject);
                        }
                    }
                }
                //else value, not escaping anything
            }

            foreach (KeyValuePair<string, string> keyValuePair in propertyNamesToReplace)
            {
                var oldPropertyName = keyValuePair.Key;
                var newPropertyName = keyValuePair.Value;
                propertyMappings.AddMapping(oldPropertyName, newPropertyName);

                var property = document.Property(oldPropertyName);
                object[] children = property.Children().ToArray();
                JProperty newJProperty;
                if (children.Length == 1)
                {
                    newJProperty = new JProperty(newPropertyName, children[0]);   
                }
                else
                {
                    newJProperty = new JProperty(newPropertyName, children);
                }
                property.Replace(newJProperty);
            }
        }
    }

    public static class SpecialCharacters
    {
        public const char EscapeChar = '_';
        public static HashSet<char> ProblematicCharacterSet = new HashSet<char>() {':', '/', '-', '#', '@', '.'};
    }

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

    public class PropertyMappings
    {
        private Dictionary<string,string> _mappings = new Dictionary<string, string>();

        public void AddMapping(string oldPropertyName, string newPropertyName)
        {
            string containedMapping;
            var contained = _mappings.TryGetValue(oldPropertyName, out containedMapping);
            if (!contained)
            {
                _mappings.Add(oldPropertyName, newPropertyName);
            }
            else
            {
                if (containedMapping != newPropertyName)
                {
                    throw new NotSupportedException(String.Format("Can't add mapping {0} to {1}, because already mapped to {2}", oldPropertyName, newPropertyName, containedMapping));
                }
            }
        }
    }
}
