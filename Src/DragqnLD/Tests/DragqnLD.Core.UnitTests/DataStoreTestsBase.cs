using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Implementations;
using Raven.Client.Embedded;

namespace DragqnLD.Core.UnitTests
{
    public abstract class DataStoreTestsBase
    {
        protected readonly IDataStore _ravenDataStore;
        protected readonly EmbeddableDocumentStore _documentStore;
        protected const string JsonBernersLeeFileName = @"JSON\berners-lee.jsonld";

        protected const int RavenWebUiPort = 8081;
        protected const string JsonBersersLeeId = @"http://www.w3.org/People/Berners-Lee/card#i";

        protected DataStoreTestsBase()
        {
            var docStore = new EmbeddableDocumentStore()
            {
                RunInMemory = true,
                Configuration = { Port = RavenWebUiPort }
            };
            Raven.Database.Server.NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(RavenWebUiPort);

            docStore.Initialize();

            _documentStore = docStore;
            _ravenDataStore = new RavenDataStore(docStore, new DocumentPropertyEscaper());
        }
    }
}