using System;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Implementations;
using Raven.Client.Embedded;
using Raven.Database.Server;
using Xunit;
using Xunit.Abstractions;

namespace DragqnLD.Core.UnitTests
{

    public class DataStoreFixture : IDisposable
    {
        public readonly IDataStore RavenDataStore;
        public readonly EmbeddableDocumentStore DocumentStore;
        
        protected const int RavenWebUiPort = 8081;

        public DataStoreFixture()
        {
            var docStore = new EmbeddableDocumentStore
            {
                RunInMemory = true,
                Configuration = { Port = RavenWebUiPort }
            };
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(RavenWebUiPort);

            docStore.Initialize();

            DocumentStore = docStore;
            RavenDataStore = new RavenDataStore(docStore, new DocumentPropertyEscaper(), new PropertyUnescapesCache());
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
    public abstract class DataStoreTestsBase : TestsBase
    {
        protected readonly IDataStore RavenDataStore;
        protected readonly EmbeddableDocumentStore DocumentStore;
        protected const string JsonBernersLeeFileName = @"JSON\berners-lee.jsonld";

        protected const string JsonBersersLeeId = @"http://www.w3.org/People/Berners-Lee/card#i";

        private readonly DataStoreFixture _dataStoreFixture;

        protected DataStoreTestsBase(ITestOutputHelper output, DataStoreFixture dataStoreFixture) : base(output)
        {
            _dataStoreFixture = dataStoreFixture;
            DocumentStore = _dataStoreFixture.DocumentStore;
            RavenDataStore = _dataStoreFixture.RavenDataStore;
        }
    }
}