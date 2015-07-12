using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Implementations;
using DragqnLD.Core.Implementations.Utils;
using DragqnLD.Core.UnitTests.Utils;
using JsonLD.Core;
using Newtonsoft.Json.Linq;
using Raven.Json.Linq;
using Raven.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace DragqnLD.Core.UnitTests
{

    [Trait("Category", "Basic")]
    //todo: get rid of Content property
    //todo: make all simple test data work with a contained @id property
    public class RavenDataStoreTests : DataStoreTestsBase, IClassFixture<DataStoreFixture>
    {
        
        public RavenDataStoreTests(ITestOutputHelper output, DataStoreFixture dataStoreFixture) : base(output, dataStoreFixture)
        {
            
        }

        [Fact]
        public async Task CanStoreAndGetPlainJsonData()
        {
            var content = RavenJObject.Parse(@"{ ""name"" : ""Petr""}");

            var dataToStore = new ConstructResult
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"),
                Document = new Document { Content = content }
            };

            await RavenDataStore.StoreDocument(dataToStore);

            RavenTestBase.WaitForUserToContinueTheTest(DocumentStore);

            var storedDocument = await RavenDataStore.GetDocument(dataToStore.QueryId, dataToStore.DocumentId);

            Assert.Equal(storedDocument.Content.ToString(), content.ToString());
        }

        [Fact]
        public async Task CanStoreAndGetComplexJsonLDData()
        {
            const string queryId = "QueryDefinitions/1";
            var storedData = await EscapeAndStoreDocument(JsonBernersLeeFileName, JsonBersersLeeId, queryId);

            RavenTestBase.WaitForUserToContinueTheTest(DocumentStore);

            var storedDocument = await RavenDataStore.GetDocument(storedData.QueryId, storedData.DocumentId);

            Assert.NotNull(storedDocument);
            Output.WriteLine("=========================================");
            Output.WriteLine("Original data:");
            Output.WriteLine(storedData.Document.Content.ToString());
            Output.WriteLine("=========================================");
            Output.WriteLine("Stored data:");
            Output.WriteLine(storedDocument.Content.ToString());

            Assert.Equal(storedData.Document.Content.ToString(), storedDocument.Content.ToString());
        }

        [Fact]
        public async Task CanQuerySimpleJsonData()
        {
            var dataToStore = new ConstructResult
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"),
                Document = new Document { Content = RavenJObject.Parse(@"{ ""@id"" : ""http://linked.opendata.cz/resource/ATC/M01AE02"", ""name"" : ""Petr""}") }
            };

            await RavenDataStore.StoreDocument(dataToStore);

            var dataToStore2 = new ConstructResult
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE02"),
                Document = new Document { Content = RavenJObject.Parse(@"{ ""@id"" : ""http://linked.opendata.cz/resource/ATC/M01AE02"", ""name"" : ""Jan""}") }
            };

            await RavenDataStore.StoreDocument(dataToStore2);
            //RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
            var results = await RavenDataStore.QueryDocumentEscapedLuceneQuery(dataToStore.QueryId, "name:Petr");

            RavenTestBase.WaitForUserToContinueTheTest(DocumentStore);

            Assert.Equal(results.Count(), 1);

        }

        [Fact]
        public async Task CanInsertDuplicateIdsToDifferencQueries()
        {
            var dataToStore = new ConstructResult
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"),
                Document = new Document { Content = RavenJObject.Parse("{ \"name\" : \"Petr\"}") }
            };

            await RavenDataStore.StoreDocument(dataToStore);

            var dataToStore2 = new ConstructResult
            {
                QueryId = "QueryDefinitions/2",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"),
                Document = new Document { Content = RavenJObject.Parse("{ \"name\" : \"Jan\"}") }
            };

            await RavenDataStore.StoreDocument(dataToStore2);
            RavenTestBase.WaitForUserToContinueTheTest(DocumentStore);
            var storedDocument = await RavenDataStore.GetDocument(dataToStore.QueryId, dataToStore.DocumentId);

            RavenTestBase.WaitForUserToContinueTheTest(DocumentStore);

            Assert.NotNull(storedDocument);
            Output.WriteLine("=========================================");
            Output.WriteLine("Original data:");
            Output.WriteLine(dataToStore.Document.Content.ToString());
            Output.WriteLine("=========================================");
            Output.WriteLine("Stored data:");
            Output.WriteLine(storedDocument.Content.ToString());

            Assert.Equal(dataToStore.Document.Content.ToString(), storedDocument.Content.ToString());

        }

        [Fact]
        public async Task CanQueryComplexJsonLDData()
        {
            const string queryId = "QueryDefinitions/1";
            var dataToStore = await EscapeAndStoreDocument(JsonBernersLeeFileName, JsonBersersLeeId, queryId);

            //todo: autoescape colons in values, dots in property names .. -- escaping colons can be workaround by wrapping in " character
            var results = await RavenDataStore.QueryDocumentEscapedLuceneQuery(dataToStore.QueryId, @"_type:""http://www.w3.org/2000/10/swap/pim/contact#Male""");

            RavenTestBase.WaitForUserToContinueTheTest(DocumentStore);

            Assert.Equal(results.Count(), 1);
        }

        [Fact]
        public async Task CanQueryByNestedPropertyItemsSimpleJsonData()
        {
            var parsed = RavenJObject.Parse(@"{ ""name"" : ""Petr"", ""parents"" : [ { ""age"" : ""45"" } ] }");
            var dataToStore = new ConstructResult
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"),
                Document = new Document { Content = parsed }
            };

            await RavenDataStore.StoreDocument(dataToStore);
            //RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
            var results = await RavenDataStore.QueryDocumentEscapedLuceneQuery(dataToStore.QueryId, @"parents,age : ""45""");
            RavenTestBase.WaitForUserToContinueTheTest(DocumentStore);


            Assert.Equal(results.Count(), 1);
        }

        [Fact]
        public async Task CanQueryByNestedPropertyItemsComplexJsonLDData()
        {
            const string queryId = "QueryDefinitions/1";
            var dataToStore = await EscapeAndStoreDocument(JsonBernersLeeFileName, JsonBersersLeeId, queryId);
            //RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
            var results = await RavenDataStore.QueryDocumentEscapedLuceneQuery(dataToStore.QueryId, @"http___www_w3_org_2000_01_rdf_schema_label : ""Tim Berners-Lee""");

            Assert.Equal(results.Count(), 1);
        }

        [Fact]
        public async Task CanQueryByUnescapedPropertyComplexJsonLDData()
        {
            const string queryId = "QueryDefinitions/1";
            var dataToStore = await EscapeAndStoreDocument(JsonBernersLeeFileName, JsonBersersLeeId, queryId);
            //RavenTestBase.WaitForUserToContinueTheTest(_documentStore);
            var results = await RavenDataStore.QueryDocumentProperties(dataToStore.QueryId, 
                @"http://www.w3.org/2000/01/rdf-schema#label".AsCondition(@"""Tim Berners-Lee"""));

            Assert.Equal(results.Count(), 1);
        }

        private async Task<ConstructResult> EscapeAndStoreDocument(string fileName, string rootId, string queryId)
        {
            var reader = new StreamReader(fileName);
            var formatter = new ExpandedJsonLDDataFormatter();
            var writer = new StringWriter();
            PropertyMappings mappings;
            //todo: Context?
            var context = ContextTestHelper.EmptyContext();
            formatter.Format(reader, writer, rootId, context, out mappings);

            var parsed = RavenJObject.Parse(writer.ToString());
            var dataToStore = new ConstructResult
            {
                QueryId = queryId,
                DocumentId = new Uri(rootId),
                Document = new Document {Content = parsed}
            };

            await RavenDataStore.StoreDocument(dataToStore);
            return dataToStore;
        }

        [Fact]
        public async Task CanQueryByMultipleUnescaepdPropertiesComplexJsonLDData()
        {
            const string queryId = "QueryDefinitions/1";
            var dataToStore = await EscapeAndStoreDocument(JsonBernersLeeFileName, JsonBersersLeeId, queryId);
            //RavenTestBase.WaitForUserToContinueTheTest(this.DocumentStore);
            var results = await RavenDataStore.QueryDocumentProperties(dataToStore.QueryId,
                @"http://www.w3.org/2000/01/rdf-schema#label".AsCondition(@"""Tim Berners-Lee"""),
                @"@type".AsCondition(@"""http://www.w3.org/2000/10/swap/pim/contact#Male"""));
            
            Assert.Equal(results.Count(), 1);
        }

        [Fact]
        public async Task CanBulkInsertData()
        {
            var dataToStore = new ConstructResult
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"),
                Document = new Document { Content = RavenJObject.Parse("{ \"name\" : \"Petr\"}") }
            };
            
            var dataToStore2 = new ConstructResult
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE02"),
                Document = new Document { Content = RavenJObject.Parse("{ \"name\" : \"Jan\"}") }
            };

            await RavenDataStore.BulkStoreDocuments(dataToStore, dataToStore2);

            var results = await RavenDataStore.QueryDocumentEscapedLuceneQuery(dataToStore.QueryId, "name:Petr");

            //RavenTestBase.WaitForUserToContinueTheTest(DocumentStore);

            Assert.Equal(results.Count(), 1);
        }
    }
}
