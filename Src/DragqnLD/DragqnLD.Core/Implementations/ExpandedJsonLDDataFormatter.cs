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
using Raven.Abstractions.Extensions;

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

        public void Format(TextReader input, TextWriter output, string rootObjectId)
        {
            //might be faster by using the strings via JsonTextReader, instead of Deserializing to JObject

            var inputString = input.ReadToEnd(); //TODO: to async

            var originalObject = JObject.Parse(inputString);

            var graphObjects = originalObject["@graph"].Children();

            var flatGraphNester = new FlatGraphNester(graphObjects, rootObjectId);

            flatGraphNester.ReadObjectsFromGraph();

            flatGraphNester.NestEverythingIntoRootObject();

            var jsonWriter = new JsonTextWriter(output);
            flatGraphNester.RootJObject.WriteTo(jsonWriter);
        }


    }
}
