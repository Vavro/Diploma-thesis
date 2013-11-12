﻿using System;
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
    }
}