using System;
using System.Collections.Generic;
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
        public void Format(TextReader input, TextWriter output, string rootObjectId)
        {
            //might be faster by using the strings via JsonTextReader, instead of Deserializing annonymous

            var inputString = input.ReadToEnd(); //TODO: to async

            var originalObject = JObject.Parse(inputString);

            var graphObjects = originalObject["@graph"].Children();

            var objects = new Dictionary<string, JObject>();
            JObject rootObject = null;

            foreach (var graphObject in graphObjects)
            {
                var id = (string)graphObject["@id"];
                var typed = graphObject as JObject;
                if (id == null || typed == null) 
                        throw new NotSupportedException(String.Format("The collection of objects in the graph property, didn't contain only objects with ids - invalid json: {0}", graphObject.ToString()));

                if (id == rootObjectId)
                {
                    rootObject = typed;
                }
                else
                {
                    objects.Add(id, typed);
                }
            }

            if (rootObject == null)
                throw new NotSupportedException(String.Format("The collection of objects in the graph property, didn't contain the object with id: {0}", rootObjectId));


        }
    }
}
