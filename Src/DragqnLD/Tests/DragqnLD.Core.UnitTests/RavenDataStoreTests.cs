using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Implementations;
using Newtonsoft.Json.Linq;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Json.Linq;
using Raven.Tests.Helpers;
using Xunit;

namespace DragqnLD.Core.UnitTests
{
    public class RavenDataStoreTests
    {
        private readonly IDataStore _ravenDataStore;
        private readonly EmbeddableDocumentStore _documentStore;

        private const int RavenWebUiPort = 8081;

        public RavenDataStoreTests()
        {
            var docStore = new EmbeddableDocumentStore()
            {
                RunInMemory = true,
                Configuration = { Port = RavenWebUiPort }
            };
            Raven.Database.Server.NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(RavenWebUiPort);

            docStore.Initialize();

            _documentStore = docStore;
             _ravenDataStore = new RavenDataStore(docStore);
        }

        [Fact]
        public async Task CanStoreAndGetPlainJSONData()
        {

            var content = RavenJObject.Parse("{ \"name\" : \"Petr\"}");

            var dataToStore = new ConstructResult()
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"),
                Document = new Document() { Content = content }
            };
            
            await _ravenDataStore.StoreDocument(dataToStore);

            //RavenTestBase.WaitForUserToContinueTheTest(_documentStore);

            var storedDocument = await _ravenDataStore.GetDocument(dataToStore.QueryId, dataToStore.DocumentId);
        }

        [Fact]
        public async Task CanQuerySimpleJSONData()
        {
            var dataToStore = new ConstructResult()
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"),
                Document = new Document() { Content = RavenJObject.Parse("{ \"name\" : \"Petr\"}") }
            };

            await _ravenDataStore.StoreDocument(dataToStore);
            
            var dataToStore2 = new ConstructResult()
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE02"),
                Document = new Document() { Content = RavenJObject.Parse("{ \"name\" : \"Jan\"}") }
            };

            await _ravenDataStore.StoreDocument(dataToStore2);

            //todo: shouldn't have to specify Content in the query
            var results = await _ravenDataStore.QueryDocumentProperty(dataToStore.QueryId, "Content.name:Petr");

            //RavenTestBase.WaitForUserToContinueTheTest(_documentStore);

            Assert.Equal(results.Count(), 1);

        }
    }
}
