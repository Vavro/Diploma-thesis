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
using Raven.Abstractions.Data;
using Raven.Abstractions.Indexing;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Listeners;
using Raven.Json.Linq;
using Raven.Tests.Helpers;
using Xunit;

namespace DragqnLD.Core.UnitTests
{
    //todo: get rid of Content property
    //todo: make all simple test data work with a contained @id property
    public class RavenDataStoreTests
    {
        private class DocumentQueryListener : IDocumentQueryListener
        {
            public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
            {
            }

            private void Action(IndexQuery indexQuery)
            {
            }
        }

        private class DocumentConversionListener : IDocumentConversionListener
        {
            public void EntityToDocument(string key, object entity, RavenJObject document, RavenJObject metadata)
            {
                return;
            }

            public void DocumentToEntity(string key, object entity, RavenJObject document, RavenJObject metadata)
            {
                return;
            }
        }

        private class ExtendedDocumentConversionListener : IExtendedDocumentConversionListener
        {
            public void BeforeConversionToDocument(string key, object entity, RavenJObject metadata)
            {
                return;
            }

            public void AfterConversionToDocument(string key, object entity, RavenJObject document, RavenJObject metadata)
            {
                return;
            }

            public void BeforeConversionToEntity(string key, RavenJObject document, RavenJObject metadata)
            {
                return;
            }

            public void AfterConversionToEntity(string key, RavenJObject document, RavenJObject metadata, object entity)
            {
                return;
            }
        }

        private class MyStoreListener : IDocumentStoreListener
        {
            public bool BeforeStore(string key, object entityInstance, RavenJObject metadata, RavenJObject original)
            {
                return true;
            }

            public void AfterStore(string key, object entityInstance, RavenJObject metadata)
            {
                
            }
        }

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

            docStore.RegisterListener(new DocumentConversionListener())
                .RegisterListener(new ExtendedDocumentConversionListener())
                .RegisterListener(new MyStoreListener());

            _documentStore = docStore;
             _ravenDataStore = new RavenDataStore(docStore);
        }

        [Fact]
        public async Task CanStoreAndGetPlainJSONData()
        {
            var content = RavenJObject.Parse(@"{ ""name"" : ""Petr""}");

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
            //todo: RavenDB hates the @ as starting character in property name - need to come around this!

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
                Document = new Document() { Content = RavenJObject.Parse(@"{ ""@id"" : ""http://linked.opendata.cz/resource/ATC/M01AE02"", ""name"" : ""Petr""}") }
            };

            await _ravenDataStore.StoreDocument(dataToStore);
            
            var dataToStore2 = new ConstructResult()
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE02"),
                Document = new Document() { Content = RavenJObject.Parse(@"{ ""@id"" : ""http://linked.opendata.cz/resource/ATC/M01AE02"", ""name"" : ""Jan""}") }
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
            var reader = new StreamReader(@"JSON\berners-lee.jsonld");
            var parsed = RavenJObject.Parse(reader.ReadToEnd());
            var dataToStore = new ConstructResult()
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"),
                Document = new Document() { Content = parsed }
            };

            await _ravenDataStore.StoreDocument(dataToStore);

            //todo: autoescape colons in values, dots in property names .. 
            var results = await _ravenDataStore.QueryDocumentProperty(dataToStore.QueryId, @"_@type:http\://www.w3.org/2000/10/swap/pim/contact#Male");

            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);

            Assert.Equal(results.Count(), 1);
        }

        [Fact]
        public async Task CanQueryByNestedPropertyItemsSimpleJSONData()
        {
            var parsed = RavenJObject.Parse(@"{ ""name"" : ""Petr"", ""parents"" : [ { ""age"" : ""45"" } ] }");
            var dataToStore = new ConstructResult()
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"),
                Document = new Document() { Content = parsed }
            };

            await _ravenDataStore.StoreDocument(dataToStore);
            //RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
            var results = await _ravenDataStore.QueryDocumentProperty(dataToStore.QueryId, @"parents,age : 45");
            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);


            Assert.Equal(results.Count(), 1);
        }

        [Fact]
        public async Task CanQueryByNestedPropertyItemsComplexJSONData()
        {
            var reader = new StreamReader(@"JSON\berners-lee.jsonld");
            var parsed = RavenJObject.Parse(reader.ReadToEnd());
            var dataToStore = new ConstructResult()
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"),
                Document = new Document() {Content = parsed}
            };

            await _ravenDataStore.StoreDocument(dataToStore);
            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
            var results = await _ravenDataStore.QueryDocumentProperty(dataToStore.QueryId, @"http___www_w3_org_2000_01_rdf_schema_label,_value : ""Tim Berners-Lee""");

            Assert.Equal(results.Count(), 1);
        }
    }
}
