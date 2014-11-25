using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;
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

        protected virtual JsonTextWriter GetJsonTextWriter(Stream stream)
        {
            return new JsonTextWriter(new StreamWriter(stream))
            {
                Formatting = Formatting.Indented
            };
        }
        
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

        protected virtual Raven.Imports.Newtonsoft.Json.JsonTextWriter GetRavenJsonTextWriter(Stream stream)
        {
            return new Raven.Imports.Newtonsoft.Json.JsonTextWriter(new StreamWriter(stream))
            {
                Formatting = Raven.Imports.Newtonsoft.Json.Formatting.Indented
            };
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }

    public class UnescapingJsonContent : JsonContent
    {
        private readonly List<PropertyEscape> _mappings;

        public UnescapingJsonContent(JToken value, List<PropertyEscape> mappings) : base(value)
        {
            _mappings = mappings;
        }

        public UnescapingJsonContent(RavenJToken value, List<PropertyEscape> mappings) : base(value)
        {
            _mappings = mappings;
        }

        protected override JsonTextWriter GetJsonTextWriter(Stream stream)
        {
            return new PropertyUnescapingJsonTextWriter(new StreamWriter(stream), _mappings)
            {
                Formatting = Formatting.Indented
            };
        }

        protected override Raven.Imports.Newtonsoft.Json.JsonTextWriter GetRavenJsonTextWriter(Stream stream)
        {
            return new RavenPropertyUnescapingJsonTextWriter(new StreamWriter(stream), _mappings)
            {
                Formatting = Raven.Imports.Newtonsoft.Json.Formatting.Indented
            };
        }
    }

    public class PropertyUnescapingJsonTextWriter : JsonTextWriter
    {
        private readonly PropertyMapForUnescape _escapedMappings;

        public PropertyUnescapingJsonTextWriter(TextWriter textWriter, List<PropertyEscape> mappings) : base(textWriter)
        {
            _escapedMappings = new PropertyMapForUnescape(mappings);
        }

        public override void WritePropertyName(string name)
        {
            var originalName = _escapedMappings.GetOriginalNameOrNull(name);
            base.WritePropertyName(originalName ?? name);
        }

        public override void WritePropertyName(string name, bool escape)
        {
            var originalName = _escapedMappings.GetOriginalNameOrNull(name);
            base.WritePropertyName(originalName ?? name, escape);
        }
    }

    public class RavenPropertyUnescapingJsonTextWriter : Raven.Imports.Newtonsoft.Json.JsonTextWriter
    {
        private readonly PropertyMapForUnescape _escapedMappings;

        public RavenPropertyUnescapingJsonTextWriter(TextWriter textWriter, List<PropertyEscape> mappings) : base(textWriter)
        {
            _escapedMappings = new PropertyMapForUnescape(mappings);
        }

        public override void WritePropertyName(string name)
        {
            var originalName = _escapedMappings.GetOriginalNameOrNull(name);
            base.WritePropertyName(originalName ?? name);
        }
    }
}