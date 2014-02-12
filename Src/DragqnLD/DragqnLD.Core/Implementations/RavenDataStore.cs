using System;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Data;
using Raven.Abstractions.Linq;
using Raven.Client;
using System.Collections.Generic;
using System.Linq;
using Raven.Abstractions.Data;
using Raven.Json.Linq;
using VDS.RDF.Query.Expressions.Primary;

namespace DragqnLD.Core.Implementations
{
    public class RavenDataStore : IDataStore
    {
        //todo: Inject session from current call - Raven session will be tied to one REST call?

        private readonly IDocumentStore Store;

        public RavenDataStore(IDocumentStore store)
        {
            Store = store;
        }
        public async Task StoreDocument(ConstructResult dataToStore)
        {
            using (var session = Store.OpenAsyncSession())
            {
                var content = dataToStore.Document.Content;

                string id = GetDocumentId(dataToStore.QueryId, dataToStore.DocumentId.AbsoluteUri);

                //Store.AsyncDatabaseCommands.PutAsync(id, new Etag(UuidType.Documents, ), content, );

                await session.StoreAsync(content, id);

                //edit the entity name, so all indexed documents for the same query are together
                var metadata = session.Advanced.GetMetadataFor(content);
                metadata["Raven-Entity-Name"] = dataToStore.QueryId;
                await session.SaveChangesAsync();
            }
        }

        private static string GetDocumentId(string queryId, string documentUri)
        {
            return String.Format("{0}/{1}", queryId, documentUri);
        }


        public async Task<Document> GetDocument(string queryId, Uri documentId)
        {
            using (var session = Store.OpenAsyncSession())
            {
                string id = GetDocumentId(queryId, documentId.AbsoluteUri);
                var storedContent = await session.LoadAsync<RavenJObject>(id);
                var storedDocument = new Document() {Id = documentId.AbsoluteUri, Content = storedContent};

                return storedDocument;
            }
        }


        public async Task<IEnumerable<Uri>> QueryDocumentProperty(string queryId, string luceneQuery)
        {
            using (var session = Store.OpenAsyncSession())
            {

                var ravenLuceneQuery = session.Advanced.AsyncLuceneQuery<dynamic>()
                    .UsingDefaultOperator(QueryOperator.And)
                    .WhereEquals("@metadata.Raven-Entity-Name",queryId)
                    .Where(luceneQuery);

                //todo: paging - raven returns max 1024 documents or something like that
                //todo: returns whole documents with metadata.. probably not necessary
                var queryResults = await ravenLuceneQuery.QueryResultAsync;

                var ids = new List<string>(queryResults.Results.Count);
                foreach (var queryResult in queryResults.Results)
                {
                    var id = queryResult["@metadata"].Value<string>("@id").Substring(queryId.Length + 1);
                    ids.Add(id);
                }

                return ids.Select(id => new Uri(id));
            }
        }
    }
}