using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using JsonLD.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raven.Abstractions.Data;

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
                    else if ((asJArray = property.Value as JArray) != null)
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
                    else if ((asJValue = property.Value as JValue) != null)
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
                var objects = new Dictionary<string, JObject>();
                foreach (var graphObject in _graphObjects)
                {
                    var id = (string)graphObject["@id"];
                    var typed = graphObject as JObject;
                    if (id == null || typed == null)
                        throw new NotSupportedException(
                            String.Format(
                                "The collection of objects in the graph property, didn't contain only objects with ids - invalid json: {0}",
                                graphObject));

                    if (id == _rootObjectId)
                    {
                        _rootJObject = typed;
                    }
                    else
                    {
                        objects.Add(id, typed);
                    }
                }

                if (_rootJObject == null)
                    throw new NotSupportedException(
                        String.Format("The collection of objects in the graph property, didn't contain the object with id: {0}",
                            _rootObjectId));

                _graphObjectsById = new ReadOnlyDictionary<string, JObject>(objects);
            }
        }

        public void Format(TextReader input, TextWriter output, string rootObjectId, Context compactionContext, ConstructQueryAccessibleProperties accessibleProperties, out PropertyMappings mappings)
        {
            //might be faster by using the strings via JsonTextReader, instead of Deserializing to JObject

            var rootJObject = NestObjects(input, rootObjectId);

            var compactedRootJObjecc = Compact(compactionContext, rootJObject);

            //now traverse properties for their type
            var accessiblePropertiesWithTypes = TraverseForTypes(accessibleProperties, compactedRootJObjecc);

            mappings = CreateMappings(compactedRootJObjecc);

            //for debugging purpusses only
            //var o = compactedRootJObjecc.ToString(Formatting.Indented);

            WriteOutput(output, compactedRootJObjecc);
        }

        private object TraverseForTypes(ConstructQueryAccessibleProperties accessibleProperties, JObject rootJObject)
        {
            var rootObjectProperty = accessibleProperties.RootProperty;
            if (rootObjectProperty == null)
            {
                return null;
                //couldn't the root be just a value? no couldn't as per creation code..
            }
            CheckObjectPropertyTypes(rootJObject, rootObjectProperty);

            //todo: return something usable.. :D
            return new object();
        }

        private void CheckObjectPropertyTypes(JObject jObject, IndexableObjectProperty checkedObjectProperty)
        {
            SetOrCheckObjectHasIdAndType(jObject, checkedObjectProperty);

            foreach (var namedIndexableProperty in checkedObjectProperty.ChildProperties)
            {
                var isValueProperty = namedIndexableProperty.Property is IndexableValueProperty;

                var propertyValue = jObject[namedIndexableProperty.AbbreviatedName];
                if (propertyValue == null)
                {
                    //this data vas optional, not present here, skip
                    continue;
                }

                if (isValueProperty)
                {
                    var valueProp = (IndexableValueProperty)namedIndexableProperty.Property;

                    bool isArray;
                    ValuePropertyType detectedValueType;
                    DetectTypeOfValueProperty(propertyValue, out isArray, out detectedValueType);
                    const bool isWrappedInArray = true;

                    SetOrCheckWrappedInArrayNotChanged(namedIndexableProperty, isWrappedInArray, propertyValue);
                    SetOrCheckDetectedValueType(valueProp, detectedValueType, propertyValue);
                }
                else //is ObjectProperty
                {
                    var objectProperty = (IndexableObjectProperty)namedIndexableProperty.Property;
                    if (propertyValue is JArray)
                    {
                        var array = (JArray)propertyValue;
                        foreach (var item in array)
                        {
                            var itemAsJObject = item as JObject;
                            if (itemAsJObject == null)
                            {
                                throw new NotSupportedException(String.Format("Array values of the object property may contain only objects {0}", propertyValue));
                            }
                            CheckObjectPropertyTypes(itemAsJObject, objectProperty);
                        }
                        const bool isWrappedInArray = true;
                        SetOrCheckWrappedInArrayNotChanged(namedIndexableProperty, isWrappedInArray, propertyValue);
                    }
                    else if (propertyValue is JObject)
                    {
                        var propertyValueAsJObject = (JObject)propertyValue;
                        CheckObjectPropertyTypes(propertyValueAsJObject, objectProperty);
                        const bool isWrappedInArray = false;
                        SetOrCheckWrappedInArrayNotChanged(namedIndexableProperty, isWrappedInArray, propertyValue);
                    }
                    else
                    {
                        throw new NotSupportedException(
                            "other JTokens than JArray and JObject aren't supported as a object proerty in data");
                    }
                }
            }
        }

        private static void SetOrCheckDetectedValueType(IndexableValueProperty valueProp, ValuePropertyType detectedValueType,
            JToken propertyValue)
        {
            if (valueProp.Type == null)
            {
                valueProp.Type = detectedValueType;
            }
            else if (valueProp.Type != detectedValueType)
            {
                throw new NotSupportedException(
                    String.Format("Data isn't homogenous, previous detected type {0}, current detected type {1}, {2}",
                        valueProp.Type, detectedValueType, propertyValue));
            }
        }

        private static void SetOrCheckObjectHasIdAndType(JObject jObject, IndexableObjectProperty checkedObjectProperty)
        {
            var id = jObject["@id"];
            var hasId = id != null;
            if (checkedObjectProperty.HasId == null)
            {
                checkedObjectProperty.HasId = hasId;
            }
            else
            {
                if (checkedObjectProperty.HasId != hasId)
                {
                    throw new NotSupportedException(
                        String.Format(
                            "Not homogenous looking data - id property appearnce mismatch previously {0}, now {1} on {2}",
                            checkedObjectProperty.HasId, hasId, jObject));
                }
            }
            var type = jObject["@type"];
            var hasType = type != null;
            if (checkedObjectProperty.HasType == null)
            {
                checkedObjectProperty.HasType = hasType;
            }
            else
            {
                if (checkedObjectProperty.HasType != hasType)
                {
                    throw new NotSupportedException(
                        String.Format(
                            "Not homogenous looking data - type property appearnce mismatch previously {0}, now {1} on {2}",
                            checkedObjectProperty.HasType, hasType, jObject));
                }
            }
        }

        private static void SetOrCheckWrappedInArrayNotChanged(NamedIndexableProperty namedIndexableProperty, bool isWrappedInArray,
            JToken objectProperty)
        {
            if (namedIndexableProperty.WrappedInArray == null)
            {
                namedIndexableProperty.WrappedInArray = isWrappedInArray;
            }
            else
            {
                if (namedIndexableProperty.WrappedInArray != isWrappedInArray)
                {
                    throw new NotSupportedException(
                        String.Format("Property wrapped in array mismatch, previusly {0}, now {1}, on {2}",
                            namedIndexableProperty.WrappedInArray, isWrappedInArray, objectProperty));
                }
            }
        }

        private void DetectTypeOfValueProperty(JToken propertyValue, out bool isArray, out ValuePropertyType valueType)
        {
            if (propertyValue is JValue)
            {
                isArray = false;
                valueType = ValuePropertyType.Value;
                return;
            }
            else if (propertyValue is JArray)
            {
                var asArray = (JArray)propertyValue;
                ValuePropertyType? elementType = null;
                foreach (var token in asArray)
                {
                    var thisElementType = DetectTypeOfArrayValueProperty(token);
                    if (elementType == null)
                    {
                        elementType = thisElementType;
                    }
                    else if (elementType != thisElementType)
                    {
                        throw new NotSupportedException(String.Format("Data arent homogenous previous type {0}, this type {1}, on token {2}", elementType, thisElementType, propertyValue));
                    }
                }
                if (!elementType.HasValue)
                {
                    throw new NotSupportedException("Array had no items, couldn't detect its type");
                }
                isArray = true;
                valueType = elementType.Value;
                return;
            }
            else if (propertyValue is JObject)
            {
                if (IsLangTaggedString((JObject)propertyValue))
                {
                    isArray = false;
                    valueType = ValuePropertyType.LanguageString;
                    return;
                }
                else
                {
                    throw new NotSupportedException(String.Format("Unsupported JObject in a Value property type {0}", propertyValue));
                }
            }
            else
            {
                throw new NotSupportedException("other JTokens than JValue, JArray and JObject aren't supported as a value property in data");
            }
        }

        private ValuePropertyType DetectTypeOfArrayValueProperty(JToken token)
        {
            //might return directly ArrayOfTypes
            if (token is JValue) //any value
            {
                return ValuePropertyType.Value;
            }
            else if (token is JObject) //should be a lang tagged string
            {
                if (IsLangTaggedString((JObject)token))
                {
                    return ValuePropertyType.LanguageString;
                };
                throw new NotSupportedException("Other than language tagged strings aren't supported as objects in an array in a value property");
            }
            else
            {
                throw new NotSupportedException(
                    "other JTokens than JValue and JObject dont make sense to be a part of a value property array");
            }
        }

        private static bool IsLangTaggedString(JObject obj)
        {
            var lang = obj["@language"] as JValue;
            var val = obj["@value"] as JValue;
            if (lang != null && val != null)
            {
                return true;
            }
            return false;
        }

        private static void WriteOutput(TextWriter output, JObject compactedRootJObjecc)
        {
            var jsonWriter = new JsonTextWriter(output);
            compactedRootJObjecc.WriteTo(jsonWriter);
        }

        private static PropertyMappings CreateMappings(JObject compactedRootJObjecc)
        {
            var propertyEscaper = new DocumentPropertyEscaper();
            propertyEscaper.EscapeDocumentProperies(compactedRootJObjecc);
            PropertyMappings mappings = propertyEscaper.PropertyMappings;
            return mappings;
        }

        private static JObject Compact(Context compactionContext, JObject rootJObject)
        {
            var jsonLdOptions = new JsonLdOptions();
            jsonLdOptions.SetCompactArrays(false);
            jsonLdOptions.SetEmbed(true);
            var compactedRootJObjecc = JsonLdProcessor.Compact(rootJObject, compactionContext, jsonLdOptions);

            //if the arrays dont get compacted the processor adds { @graph [ { to the root of the doc - delete it
            compactedRootJObjecc = (JObject)compactedRootJObjecc.First.First.First;

            //done: write in @context - where it can be found
            //solved by injecting in document retrieval
            return compactedRootJObjecc;
        }

        private static JObject NestObjects(TextReader input, string rootObjectId)
        {
            var inputString = input.ReadToEnd(); //TODO: to async input reader might come from web? 

            var originalObject = JObject.Parse(inputString);

            var graphObjects = originalObject["@graph"].Children();

            var flatGraphNester = new FlatGraphNester(graphObjects, rootObjectId);

            flatGraphNester.ReadObjectsFromGraph();

            flatGraphNester.NestEverythingIntoRootObject();

            var rootJObject = flatGraphNester.RootJObject;
            return rootJObject;
        }
    }
}
