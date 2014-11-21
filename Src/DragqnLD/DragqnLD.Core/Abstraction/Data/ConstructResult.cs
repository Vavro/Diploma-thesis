using System;
using System.Collections;
using System.Collections.Generic;
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

    public class DocumentMetadata
    {
        public string Id { get; set; }
    }

    public class PagedDocumentMetadata
    {
        public IList<DocumentMetadata> Items { get; set; }

        public int TotalItems { get; set; }
    }
}
