using System.Collections.Generic;
using System.IO;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raven.Json.Linq;

namespace DragqnLD.WebApi.Connection
{
    /// <summary>
    /// To write plain json to HttpResponseMessage with unescaping property names
    /// </summary>
    public class UnescapingJsonContent : JsonContent
    {
        private readonly PropertyMapForUnescape _mappings;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnescapingJsonContent"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="mappings">The mappings.</param>
        public UnescapingJsonContent(JToken value, PropertyMapForUnescape mappings) : base(value)
        {
            _mappings = mappings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnescapingJsonContent"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="mappings">The mappings.</param>
        public UnescapingJsonContent(RavenJToken value, PropertyMapForUnescape mappings) : base(value)
        {
            _mappings = mappings;
        }

        /// <summary>
        /// Gets the json text writer.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        protected override JsonTextWriter GetJsonTextWriter(Stream stream)
        {
            return new PropertyUnescapingJsonTextWriter(new StreamWriter(stream), _mappings)
            {
                Formatting = Formatting.Indented
            };
        }

        /// <summary>
        /// Gets the raven json text writer.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        protected override Raven.Imports.Newtonsoft.Json.JsonTextWriter GetRavenJsonTextWriter(Stream stream)
        {
            return new RavenPropertyUnescapingJsonTextWriter(new StreamWriter(stream), _mappings)
            {
                Formatting = Raven.Imports.Newtonsoft.Json.Formatting.Indented
            };
        }
    }
}