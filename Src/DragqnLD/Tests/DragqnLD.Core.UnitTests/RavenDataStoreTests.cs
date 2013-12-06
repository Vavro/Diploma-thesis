using System;
using System.Collections.Generic;
using System.IO;
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
    //todo: get rid of Content property
    //todo: make all simple test data work with a contained @id property
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

            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);

            var storedDocument = await _ravenDataStore.GetDocument(dataToStore.QueryId, dataToStore.DocumentId);

            Assert.Equal(storedDocument.Content.ToString(), content.ToString());
        }

        [Fact]
        public async Task CanStoreAndGetComplexJSONData()
        {
            var reader = new StreamReader(@"JSON\berners-lee.jsonld");
            var parsed = RavenJObject.Parse(reader.ReadToEnd());
            var dataToStore = new ConstructResult()
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"),
                Document = new Document() {Content = parsed }
            };

            await _ravenDataStore.StoreDocument(dataToStore);

            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);

            var storedDocument = await _ravenDataStore.GetDocument(dataToStore.QueryId, dataToStore.DocumentId);

            Assert.NotNull(storedDocument);
            Console.WriteLine("=========================================");
            Console.WriteLine("Original data:");
            Console.WriteLine(dataToStore.Document.Content.ToString());
            Console.WriteLine("=========================================");
            Console.WriteLine("Stored data:");
            Console.WriteLine(storedDocument.Content.ToString());

            Assert.Equal(dataToStore.Document.Content.ToString(), storedDocument.Content.ToString());
        }

        [Fact]
        public async Task CanQuerySimpleJSONData()
        {
            var dataToStore = new ConstructResult()
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"),
                Document = new Document() { Content = RavenJObject.Parse("{ \"@id\" : \""+@"http://linked.opendata.cz/resource/ATC/M01AE02"+ "\", \"name\" : \"Petr\"}") }
            };

            await _ravenDataStore.StoreDocument(dataToStore);
            
            var dataToStore2 = new ConstructResult()
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE02"),
                Document = new Document() { Content = RavenJObject.Parse("{ \"@id\" : \""+@"http://linked.opendata.cz/resource/ATC/M01AE02"+ "\", \"name\" : \"Jan\"}") }
            };

            await _ravenDataStore.StoreDocument(dataToStore2);
            //RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
            var results = await _ravenDataStore.QueryDocumentProperty(dataToStore.QueryId, "name:Petr");

            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);

            Assert.Equal(results.Count(), 1);

        }

        [Fact]
        public async Task CanInsertDuplicateIdsToDifferencQueries()
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
                QueryId = "QueryDefinitions/2",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"),
                Document = new Document() { Content = RavenJObject.Parse("{ \"name\" : \"Jan\"}") }
            };

            await _ravenDataStore.StoreDocument(dataToStore2);
            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
            var storedDocument = await _ravenDataStore.GetDocument(dataToStore.QueryId, dataToStore.DocumentId);

            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);

            Assert.NotNull(storedDocument);
            Console.WriteLine("=========================================");
            Console.WriteLine("Original data:");
            Console.WriteLine(dataToStore.Document.Content.ToString());
            Console.WriteLine("=========================================");
            Console.WriteLine("Stored data:");
            Console.WriteLine(storedDocument.Content.ToString());

            Assert.Equal(dataToStore.Document.Content.ToString(), storedDocument.Content.ToString());

        }

        [Fact]
        public async Task CanQueryComplexJSONData()
        {
            
        }
    }
}
