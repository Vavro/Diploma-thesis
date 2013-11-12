using System;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Data;
using Raven.Client;
using System.Collections.Generic;
using System.Linq;
using Raven.Abstractions.Data;

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
                await session.StoreAsync(dataToStore.Document, dataToStore.DocumentId.AbsoluteUri);

                //edit the entity name, so all indexed documents for the same query are together
                var metadata = session.Advanced.GetMetadataFor(dataToStore.Document);
                metadata["Raven-Entity-Name"] = dataToStore.QueryId;
                await session.SaveChangesAsync();
            }
        }

        public async Task<Document> GetDocument(string queryId, Uri documentId)
        {
            using (var session = Store.OpenAsyncSession())
            {
                var storedDocument = await session.LoadAsync<Document>(documentId.AbsoluteUri);
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

                //todo: returns whole documents.. probably not necessary
                var queryResults = await ravenLuceneQuery.ToListAsync();

                return queryResults.Cast<Document>().Select(doc => new Uri(doc.Id));
            }
        }
    }
}