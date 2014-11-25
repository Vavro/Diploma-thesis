using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raven.Abstractions.Extensions;
using Raven.Json.Linq;

namespace DragqnLD.WebApi.Connection
{
    /// <summary>
    /// To write plain json to HttpResponseMessage
    /// </summary>
    // from: http://www.bizcoder.com/returning-raw-json-content-from-asp-net-web-api
    public class JsonContent : HttpContent
    {
        private readonly JToken _value;
        private readonly RavenJToken _ravenValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonContent"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonContent(JToken value)
        {
            _value = value;
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonContent"/> class.
        /// </summary>
        /// <param name="value">The RavenJToken value.</param>
        public JsonContent(RavenJToken value)
        {
            _ravenValue = value;
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        /// <summary>
        /// Gets the json text writer.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        protected virtual JsonTextWriter GetJsonTextWriter(Stream stream)
        {
            return new JsonTextWriter(new StreamWriter(stream))
            {
                Formatting = Formatting.Indented
            };
        }

        /// <summary>
        /// Serialize the HTTP content to a stream as an asynchronous operation.
        /// </summary>
        /// <param name="stream">The target stream.</param>
        /// <param name="context">Information about the transport (channel binding token, for example). This parameter may be null.</param>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task" />.The task object representing the asynchronous operation.
        /// </returns>
        protected override Task SerializeToStreamAsync(Stream stream,
            TransportContext context)
        {

            if (_value != null)
            {
                var jw = GetJsonTextWriter(stream);
                _value.WriteTo(jw);

                jw.Flush();
            }
            else
            {
                var rjw = GetRavenJsonTextWriter(stream);
                

                _ravenValue.WriteTo(rjw, null);

                rjw.Flush();
            }
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Gets the raven specific json text writer.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        protected virtual Raven.Imports.Newtonsoft.Json.JsonTextWriter GetRavenJsonTextWriter(Stream stream)
        {
            return new Raven.Imports.Newtonsoft.Json.JsonTextWriter(new StreamWriter(stream))
            {
                Formatting = Raven.Imports.Newtonsoft.Json.Formatting.Indented
            };
        }

        /// <summary>
        /// Determines whether the HTTP content has a valid length in bytes.
        /// </summary>
        /// <param name="length">The length in bytes of the HTTP content.</param>
        /// <returns>
        /// Returns <see cref="T:System.Boolean" />.true if <paramref name="length" /> is a valid length; otherwise, false.
        /// </returns>
        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }
}