using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Implementations;
using DragqnLD.Core.Implementations.Utils;
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
        private readonly IDataStore _ravenDataStore;
        private readonly EmbeddableDocumentStore _documentStore;
        private const string JsonBernersLeeFileName = @"JSON\berners-lee.jsonld";

        private const int RavenWebUiPort = 8081;
        private const string JsonBersersLeeId = @"http://www.w3.org/People/Berners-Lee/card#i";

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
            _ravenDataStore = new RavenDataStore(docStore, new DocumentPropertyEscaper());
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
        public async Task CanStoreAndGetComplexJSONLDData()
        {
            //todo: RavenDB hates the @ as starting character in property name - need to come around this!
            var reader = new StreamReader(JsonBernersLeeFileName);
            var formatter = new ExpandedJsonLDDataFormatter();
            var formattedStream = new MemoryStream();
            var writer = new StreamWriter(formattedStream);
            PropertyMappings mappings;
            formatter.Format(reader, writer, JsonBersersLeeId, out mappings);
            writer.Flush();
            formattedStream.Position = 0;
            var formattedReader = new StreamReader(formattedStream);

            var parsed = RavenJObject.Parse(formattedReader.ReadToEnd());
            var dataToStore = new ConstructResult()
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"),
                Document = new Document() { Content = parsed }
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
            var results = await _ravenDataStore.QueryDocumentEscapedLuceneQuery(dataToStore.QueryId, "name:Petr");

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
        public async Task CanQueryComplexJSONLSData()
        {
            var queryId = "QueryDefinitions/1";
            var dataToStore = await EscapeAndStoreDocument(JsonBernersLeeFileName, JsonBersersLeeId, queryId);

            //todo: autoescape colons in values, dots in property names .. -- escaping colons can be workaround by wrapping in " character
            var results = await _ravenDataStore.QueryDocumentEscapedLuceneQuery(dataToStore.QueryId, @"_type:""http://www.w3.org/2000/10/swap/pim/contact#Male""");

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
            var results = await _ravenDataStore.QueryDocumentEscapedLuceneQuery(dataToStore.QueryId, @"parents,age : ""45""");
            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);


            Assert.Equal(results.Count(), 1);
        }

        [Fact]
        public async Task CanQueryByNestedPropertyItemsComplexJSONLDData()
        {
            var queryId = "QueryDefinitions/1";
            var dataToStore = await EscapeAndStoreDocument(JsonBernersLeeFileName, JsonBersersLeeId, queryId);
            //RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
            var results = await _ravenDataStore.QueryDocumentEscapedLuceneQuery(dataToStore.QueryId, @"http___www_w3_org_2000_01_rdf_schema_label,_value : ""Tim Berners-Lee""");

            Assert.Equal(results.Count(), 1);
        }

        [Fact]
        public async Task CanQueryByUnescapedPropertyComplexJSONLDData()
        {
            var queryId = "QueryDefinitions/1";
            var dataToStore = await EscapeAndStoreDocument(JsonBernersLeeFileName, JsonBersersLeeId, queryId);
            //RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
            var results = await _ravenDataStore.QueryDocumentProperties(dataToStore.QueryId, 
                @"http://www.w3.org/2000/01/rdf-schema#label,@value".AsCondition(@"""Tim Berners-Lee"""));

            Assert.Equal(results.Count(), 1);
        }

        private async Task<ConstructResult> EscapeAndStoreDocument(string fileName, string rootId, string queryId)
        {
            var reader = new StreamReader(fileName);
            var formatter = new ExpandedJsonLDDataFormatter();
            var formattedStream = new MemoryStream();
            var writer = new StreamWriter(formattedStream);
            PropertyMappings mappings;
            formatter.Format(reader, writer, rootId, out mappings);
            writer.Flush();
            formattedStream.Position = 0;
            var formattedReader = new StreamReader(formattedStream);

            var parsed = RavenJObject.Parse(formattedReader.ReadToEnd());
            var dataToStore = new ConstructResult()
            {
                QueryId = queryId,
                DocumentId = new Uri(rootId),
                Document = new Document() {Content = parsed}
            };

            await _ravenDataStore.StoreDocument(dataToStore);
            return dataToStore;
        }

        [Fact]
        public async Task CanQueryByMultipleUnescaepdPropertiesComplexJSONLDData()
        {
            var queryId = "QueryDefinitions/1";
            var dataToStore = await EscapeAndStoreDocument(JsonBernersLeeFileName, JsonBersersLeeId, queryId);
            //RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
            var results = await _ravenDataStore.QueryDocumentProperties(dataToStore.QueryId,
                @"http://www.w3.org/2000/01/rdf-schema#label,@value".AsCondition(@"""Tim Berners-Lee"""),
                @"@type".AsCondition(@"""http://www.w3.org/2000/10/swap/pim/contact#Male"""));
            
            Assert.Equal(results.Count(), 1);
        }

        [Fact]
        public async Task CanBulkInsertData()
        {
            var dataToStore = new ConstructResult()
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"),
                Document = new Document() { Content = RavenJObject.Parse("{ \"name\" : \"Petr\"}") }
            };
            
            var dataToStore2 = new ConstructResult()
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE02"),
                Document = new Document() { Content = RavenJObject.Parse("{ \"name\" : \"Jan\"}") }
            };

            await _ravenDataStore.BulkStoreDocuments(dataToStore, dataToStore2);

            var results = await _ravenDataStore.QueryDocumentEscapedLuceneQuery(dataToStore.QueryId, "name:Petr");

            RavenTestBase.WaitForUserToContinueTheTest(_documentStore);

            Assert.Equal(results.Count(), 1);
        }
    }
}
