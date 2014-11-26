using System.Collections.Generic;
using System.IO;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;

namespace DragqnLD.WebApi.Connection
{
    /// <summary>
    /// Raven specific Json text writer unescaping property names
    /// </summary>
    public class RavenPropertyUnescapingJsonTextWriter : Raven.Imports.Newtonsoft.Json.JsonTextWriter
    {
        private readonly PropertyMapForUnescape _escapedMappings;

        /// <summary>
        /// Initializes a new instance of the <see cref="RavenPropertyUnescapingJsonTextWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="mappings">The mappings.</param>
        public RavenPropertyUnescapingJsonTextWriter(TextWriter textWriter, PropertyMapForUnescape mappings) : base(textWriter)
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
    }
}