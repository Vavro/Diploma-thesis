using System;
using System.Collections.Generic;
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

        protected override Task SerializeToStreamAsync(Stream stream,
            TransportContext context)
        {

            if (_value != null)
            {
                var jw = new JsonTextWriter(new StreamWriter(stream))
                {
                    Formatting = Formatting.Indented
                };
                _value.WriteTo(jw);

                jw.Flush();
            }
            else
            {
                var rjw = new Raven.Imports.Newtonsoft.Json.JsonTextWriter(new StreamWriter(stream))
                {
                    Formatting = Raven.Imports.Newtonsoft.Json.Formatting.Indented
                };
                

                _ravenValue.WriteTo(rjw, null);

                rjw.Flush();
            }
            return Task.FromResult<object>(null);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }
}