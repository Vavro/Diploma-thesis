using System;
using Raven.Json.Linq;

namespace DragqnLD.Core.Abstraction.Data
{
    public class ConstructResult
    {
        //todo: query id as URI
        public string QueryId { get; set; }
        public Document Document { get; set; }
        public Uri DocumentId { get; set; }
    }

    public class Document
    {
        public RavenJObject Content { get; set; }
    }
}
