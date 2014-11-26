using System.Collections.Generic;
using System.IO;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;
using Newtonsoft.Json;

namespace DragqnLD.WebApi.Connection
{
    /// <summary>
    /// Json text writer unescaping property names
    /// </summary>
    public class PropertyUnescapingJsonTextWriter : JsonTextWriter
    {
        private readonly PropertyMapForUnescape _escapedMappings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyUnescapingJsonTextWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="mappings">The mappings.</param>
        public PropertyUnescapingJsonTextWriter(TextWriter textWriter, PropertyMapForUnescape mappings) : base(textWriter)
        {
            _escapedMappings = mappings;
        }

        /// <summary>
        /// Writes the property name of a name/value pair on a Json object.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        public override void WritePropertyName(string name)
        {
            var originalName = _escapedMappings.GetOriginalNameOrNull(name);
            base.WritePropertyName(originalName ?? name);
        }

        /// <summary>
        /// Writes the property name of a name/value pair on a JSON object.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="escape">A flag to indicate whether the text should be escaped when it is written as a JSON property name.</param>
        public override void WritePropertyName(string name, bool escape)
        {
            var originalName = _escapedMappings.GetOriginalNameOrNull(name);
            base.WritePropertyName(originalName ?? name, escape);
        }
    }
}