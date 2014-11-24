using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace DragqnLD.WebApi.MediaFormatters
{
    public class JsonNetFormatter : MediaTypeFormatter
    {
        private JsonSerializerSettings _jsonSerializerSettings;



        public JsonNetFormatter(JsonSerializerSettings jsonSerializerSettings)
        {
            _jsonSerializerSettings = jsonSerializerSettings ?? new JsonSerializerSettings();

            // Fill out the mediatype and encoding we support
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            SupportedEncodings.Add(new UTF8Encoding(false, true));
        }

        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContent content, IFormatterLogger formatterLogger)
        {
            // Create a serializer
            JsonSerializer serializer = JsonSerializer.Create(_jsonSerializerSettings);

            // Create task reading the content
            return Task.Factory.StartNew(() =>
            {
                using (StreamReader streamReader = new StreamReader(stream, SupportedEncodings.First()))
                {
                    using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                    {
                        return serializer.Deserialize(jsonTextReader, type);
                    }
                }
            });
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent content, TransportContext context)
        {
            // Create a serializer
            JsonSerializer serializer = JsonSerializer.Create(_jsonSerializerSettings);

            // Create task writing the serialized content
            return Task.Factory.StartNew(() =>
            {
                using (JsonTextWriter jsonTextWriter = new JsonTextWriter(new StreamWriter(stream, SupportedEncodings.First())) { CloseOutput = false })
                {
                    serializer.Serialize(jsonTextWriter, value);
                    jsonTextWriter.Flush();
                }
            });
        }
    }
}