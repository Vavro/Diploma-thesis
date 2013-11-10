using System;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Data;
using Raven.Client;

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

                var metadata = session.Advanced.GetMetadataFor(dataToStore.Document);
                metadata["Raven-Entity-Name"] = dataToStore.QueryId;
                await session.SaveChangesAsync();
            }
        }

        public async Task<dynamic> GetDocument(string queryId, Uri documentId)
        {
            using (var session = Store.OpenAsyncSession())
            {
                var storedDocument = await session.LoadAsync<dynamic>(documentId.AbsoluteUri);
                return storedDocument;
            }
        }
    }
}