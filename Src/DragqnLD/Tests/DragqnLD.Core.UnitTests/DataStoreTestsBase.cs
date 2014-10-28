using System;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Implementations;
using Raven.Client.Embedded;
using Raven.Database.Server;

namespace DragqnLD.Core.UnitTests
{
    public abstract class DataStoreTestsBase : IDisposable
    {
        protected readonly IDataStore RavenDataStore;
        protected readonly EmbeddableDocumentStore DocumentStore;
        protected const string JsonBernersLeeFileName = @"JSON\berners-lee.jsonld";

        protected const int RavenWebUiPort = 8081;
        protected const string JsonBersersLeeId = @"http://www.w3.org/People/Berners-Lee/card#i";

        protected DataStoreTestsBase()
        {
            var docStore = new EmbeddableDocumentStore
            {
                RunInMemory = true,
                Configuration = { Port = RavenWebUiPort }
            };
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(RavenWebUiPort);

            docStore.Initialize();

            DocumentStore = docStore;
            RavenDataStore = new RavenDataStore(docStore, new DocumentPropertyEscaper());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                DocumentStore.Dispose();
                
                GC.Collect(2);
                GC.WaitForPendingFinalizers();
            }
        }
    }
}