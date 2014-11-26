using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;

namespace DragqnLD.Core.Abstraction
{
    /// <summary>
    /// Data store interface
    /// </summary>
    public interface IDataStore
    {
        Task StoreDocument(ConstructResult dataToStore);

        Task<Document> GetDocument(string queryId, Uri documentId);

        Task<Tuple<Document, PropertyMapForUnescape>> GetDocumentWithMappings(string queryId, Uri documentId, bool loadMappings = false);

        Task<IEnumerable<Uri>> QueryDocumentEscapedLuceneQuery(string queryId, string indexName,
            string luceneQuery);

        Task<IEnumerable<Uri>> QueryDocumentEscapedLuceneQuery(string queryId, string luceneQuery);

        Task<IEnumerable<Uri>> QueryDocumentProperties(string queryId, 
            params PropertyCondition[] conditions);
        Task<IEnumerable<Uri>> QueryDocumentProperties(string queryId, string indexName = null,
            params PropertyCondition[] conditions);

        Task BulkStoreDocuments(IEnumerable<ConstructResult> results);
        
        //todo: maybe not necessary, delete?
        Task BulkStoreDocuments(params ConstructResult[] results);
        Task<PagedDocumentMetadata> GetDocuments(string definitionId, int start = 0, int pageSize = 20);
    }

}

