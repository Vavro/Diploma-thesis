﻿using System.Collections.Generic;
using System.Linq;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Implementations.Utils;
using Newtonsoft.Json.Linq;

namespace DragqnLD.Core.Implementations
{
    //todo: should be parametrized by settings from construct query analysis
    //query analysis should give out the replacement strings
    public class DocumentPropertyEscaper : IDocumentPropertyEscaper
    {
        private readonly PropertyMappings _propertyMappings = new PropertyMappings();
        public PropertyMappings PropertyMappings { get { return _propertyMappings; } }

        public void EscapeDocumentProperies(JObject document)
        {
            EscapePropertiesInJObject(document);
        }

        public string EscapePropertyPath(string propertyPath)
        {
            //might in future consider the property mappings

            string output;
            propertyPath.ReplaceChars(SpecialCharacters.ProblematicCharacterSet, SpecialCharacters.EscapeChar,
                out output);
            return output;
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
                _propertyMappings.AddMapping(oldPropertyName, newPropertyName);

                var property = document.Property(oldPropertyName);
                // ReSharper disable once CoVariantArrayConversion
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
}