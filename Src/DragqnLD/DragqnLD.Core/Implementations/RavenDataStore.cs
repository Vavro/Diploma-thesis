using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Abstraction.Query;
using Raven.Client;
using System.Collections.Generic;
using System.Linq;
using Raven.Json.Linq;

namespace DragqnLD.Core.Implementations
{
    public class RavenDataStore : IDataStore
    {
        //todo: Inject session from current call - Raven session will be tied to one REST call?

        private readonly IDocumentStore _store;

        private readonly IDocumentPropertyEscaper _escaper;
        private readonly IPropertyUnescapesCache _propertyUnescapesCache;

        public RavenDataStore(IDocumentStore store, IDocumentPropertyEscaper escaper, IPropertyUnescapesCache propertyUnescapesCache )
        {
            _store = store;
            _escaper = escaper;
            _propertyUnescapesCache = propertyUnescapesCache;
        }

        public async Task StoreDocument(ConstructResult dataToStore)
        {
            using (var session = _store.OpenAsyncSession())
            {
                var content = dataToStore.Document.Content;

                string id = GetDocumentId(dataToStore.QueryId, dataToStore.DocumentId.AbsoluteUri);

                //_store.AsyncDatabaseCommands.PutAsync(id, new Etag(UuidType.Documents, ), content, );

                await session.StoreAsync(content, id).ConfigureAwait(false);

                //edit the entity name, so all indexed documents for the same query are together
                var metadata = session.Advanced.GetMetadataFor(content);
                metadata["Raven-Entity-Name"] = dataToStore.QueryId;
                await session.SaveChangesAsync().ConfigureAwait(false);
            }
        }
        
        public async Task BulkStoreDocuments(IEnumerable<ConstructResult> results)
        {
            using(var bulkInsert = _store.BulkInsert())
            { 
                foreach (ConstructResult constructResult in results)
                {
                    var document = constructResult.Document.Content;
                    string id = GetDocumentId(constructResult.QueryId, constructResult.DocumentId.AbsoluteUri);
                    var metadata = RavenJObject.Parse(String.Format(@"{{""Raven-Entity-Name"" : ""{0}""}}", constructResult.QueryId));
                    bulkInsert.Store(document, metadata, id);
                }

                await bulkInsert.DisposeAsync().ConfigureAwait(false);
            }

        }

        public Task BulkStoreDocuments(params ConstructResult[] results)
        {
            return BulkStoreDocuments(results.AsEnumerable());
        }

        public async Task<PagedDocumentMetadata> GetDocuments(string definitionId, int start = 0, int pageSize = 20)
        {
            using (var session = _store.OpenAsyncSession())
            {
                RavenQueryStatistics statistics;
                var queryResults = await session.Advanced.AsyncLuceneQuery<dynamic>()
                    .Statistics(out statistics)
                    .WhereEquals("@metadata.Raven-Entity-Name", definitionId)
                    .Skip(start)
                    .Take(pageSize)
                    .SelectFields<dynamic>("@metadata.@id")
                    .QueryResultAsync
                    .ConfigureAwait(false);

                var documentMetadatas = new List<DocumentMetadata>(queryResults.Results.Count);
                documentMetadatas.AddRange(queryResults.Results.Select(queryResult => new DocumentMetadata() {Id = queryResult.Value<string>("__document_id").Substring(definitionId.Length + 1)}));
                
                return new PagedDocumentMetadata() {Items = documentMetadatas, TotalItems = statistics.TotalResults};
            }
        }

        private static string GetDocumentId(string queryId, string documentUri)
        {
            return String.Format("{0}/{1}", queryId, documentUri);
        }

        public async Task<Document> GetDocument(string queryId, Uri documentId)
        {
            return (await GetDocumentWithMappings(queryId, documentId)).Item1;
        }

        public async Task<Tuple<Document, PropertyMapForUnescape>> GetDocumentWithMappings(string queryId, Uri documentId)
        {
            using (var session = _store.OpenAsyncSession())
            {
                var mappings = await _propertyUnescapesCache.GetMapForUnescape(queryId, async () =>
                {
                    // function has to be awaited if run, should access after using block
                    // ReSharper disable AccessToDisposedClosure
                    Debug.Assert(session != null, "session != null");
                    var qd = await session.LoadAsync<QueryDefinition>(queryId);
                    // ReSharper restore AccessToDisposedClosure

                    return qd.Mappings;
                });

                string id = GetDocumentId(queryId, documentId.AbsoluteUri);
                var storedContent = await session.LoadAsync<RavenJObject>(id).ConfigureAwait(false);
                var storedDocument = new Document { Content = storedContent };



                return new Tuple<Document, PropertyMapForUnescape>(storedDocument, mappings);
            }
        }

        public async Task<IEnumerable<Uri>> QueryDocumentProperties(string queryId, 
            params PropertyCondition[] conditions)
        {
            return await QueryDocumentProperties(queryId, null, conditions).ConfigureAwait(false);
        }

        //todo: consider extracting path escaping interface, breaks SRP a bit, reformat of documents isn't in the DataStore either
        public async Task<IEnumerable<Uri>> QueryDocumentProperties(string queryId, string indexName,
            params PropertyCondition[] conditions)
        {
            var luceneQuery = new StringBuilder();
            foreach (PropertyCondition propertyCondition in conditions)
            {
                var escapedPropertyPath = _escaper.EscapePropertyPath(propertyCondition.PropertyPath);

                //todo: add support for multiple values - @in<Property>:(value1, value2) 
                if (luceneQuery.Length > 0)
                {
                    luceneQuery.Append(" AND ");
                }
                luceneQuery.Append("(")
                    .Append(escapedPropertyPath)
                    .Append(" : ")
                    .Append(propertyCondition.Value)
                    .Append(")");
            }
            return await QueryDocumentEscapedLuceneQuery(queryId, indexName, luceneQuery.ToString()).ConfigureAwait(false);
        }


        public async Task<IEnumerable<Uri>> QueryDocumentEscapedLuceneQuery(string queryId, string luceneQuery)
        {
            return await QueryDocumentEscapedLuceneQuery(queryId, null, luceneQuery).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Uri>> QueryDocumentEscapedLuceneQuery(string queryId, string indexName, string luceneQuery)
        {
            using (var session = _store.OpenAsyncSession())
            {
                IAsyncDocumentQuery<dynamic> ravenLuceneQuery;
                if (indexName != null)
                {
                    ravenLuceneQuery = session.Advanced.AsyncLuceneQuery<dynamic>(indexName).WhereEquals("_metadata_Raven_Entity_Name", queryId);
                }
                else
                {
                    //todo: perf test version .AsyncLuceneQuery<dynamic>(String.Format("dynamic/{0}", queryId)); hates "/" in queryId but could be faster when more collections are in db
                    ravenLuceneQuery = session.Advanced.AsyncLuceneQuery<dynamic>().WhereEquals("@metadata.Raven-Entity-Name", queryId);
                }

                ravenLuceneQuery = ravenLuceneQuery
                    .AndAlso()
                    .Where(luceneQuery)
                    .SelectFields<dynamic>("@metadata.@id")
                    ;

                //todo: paging - raven returns max 1024 documents or something like that
                var queryResults = await ravenLuceneQuery.QueryResultAsync.ConfigureAwait(false);

                var ids = new List<string>(queryResults.Results.Count);
                foreach (var queryResult in queryResults.Results)
                {
                    string id = queryResult.Value<string>("__document_id").Substring(queryId.Length + 1);
                    ids.Add(id);
                }

                return ids.Select(id => new Uri(id));
            }
        }

    }
}