using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using DragqnLD.Core.Abstraction.Indexes;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;
using JsonLD.Core;
using Newtonsoft.Json.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Linq;
using Raven.Database.Config;
using Raven.Database.Indexing;
using Raven.Json.Linq;
using Raven.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace DragqnLD.Core.UnitTests
{
    public class QueryStoreFixture : RavenTestBase, IDisposable
    {
        public readonly QueryStore QueryStore;
        public readonly EmbeddableDocumentStore DocumentStore;

        protected const int RavenWebUiPort = 8081;

        protected override void ModifyConfiguration(InMemoryRavenConfiguration configuration)
        {
            base.ModifyConfiguration(configuration);
            configuration.Storage.Voron.AllowOn32Bits = true;

        }

        public QueryStoreFixture()
        {
            //NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(RavenWebUiPort);
            var docStore = NewDocumentStore(port: RavenWebUiPort);

            DocumentStore = docStore;
            QueryStore = new QueryStore(docStore);
        }

        public override void Dispose()
        {
            Dispose(true);

            base.Dispose();
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


    [Trait("Category", "Basic")]
    public class QueryStoreTests : TestsBase, IClassFixture<QueryStoreFixture>
    {
        public QueryStoreTests(ITestOutputHelper output, QueryStoreFixture queryStoreFixture)
            : base(output)
        {
            _documentStore = queryStoreFixture.DocumentStore;
            _queryStore = queryStoreFixture.QueryStore;
        }

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly EmbeddableDocumentStore _documentStore;
        private readonly QueryStore _queryStore;

        [Fact]
        public async Task CanStoreQuery()
        {
            var queryDefinition = new QueryDefinition
            {
                Name = "test query",
                Description = "testing query",
                ConstructQuery = new SparqlQueryInfo
                {
                    Query = @"DESCRIBE <http://linked.opendata.cz/resource/ATC/M01AE01>",
                    DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                    SparqlEndpoint = new Uri(@"http://linked.opendata.cz/sparql")
                },
                ConstructQueryUriParameterName = "test",
                SelectQuery = new SparqlQueryInfo
                {
                    Query = @"SELECT DISTINCT ?s WHERE { ?s ?p ?o }",
                    DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                    SparqlEndpoint = new Uri(@"http://linked.opendata.cz/sparql")
                }
            };

            var id = await _queryStore.Add(queryDefinition);
            Output.WriteLine(id);

            var retrievedQueryDefinition = await _queryStore.Get(id);

            Assert.Equal(queryDefinition.Name, retrievedQueryDefinition.Name);
            Assert.Equal(queryDefinition.Description, retrievedQueryDefinition.Description);
            Assert.Equal(queryDefinition.ConstructQuery.Query, retrievedQueryDefinition.ConstructQuery.Query);
            Assert.Equal(queryDefinition.ConstructQuery.DefaultDataSet, retrievedQueryDefinition.ConstructQuery.DefaultDataSet);
            Assert.Equal(queryDefinition.ConstructQuery.SparqlEndpoint, retrievedQueryDefinition.ConstructQuery.SparqlEndpoint);
            Assert.Equal(queryDefinition.ConstructQueryUriParameterName, retrievedQueryDefinition.ConstructQueryUriParameterName);
            Assert.Equal(queryDefinition.SelectQuery.Query, retrievedQueryDefinition.SelectQuery.Query);
            Assert.Equal(queryDefinition.SelectQuery.DefaultDataSet, retrievedQueryDefinition.SelectQuery.DefaultDataSet);
            Assert.Equal(queryDefinition.SelectQuery.SparqlEndpoint, retrievedQueryDefinition.SelectQuery.SparqlEndpoint);
        }


        [Fact]
        public async Task CanStoreAndLoadContext()
        {
            var context = new RavenJObject();
            var contextContent = new RavenJObject();
            context.Add("@context", contextContent);

            contextContent.Add("enc", new RavenJValue("http://http://linked.opendata.cz/ontology/drug-encyclopedia/"));
            contextContent.Add("skos", new RavenJValue("http://www.w3.org/2004/02/skos/core"));

            const string definitionId = "Query/1";

            var compactionContext = new CompactionContext(context, new Dictionary<string, string>()); //uri mappings to abbreviations not needed

            await _queryStore.StoreContext(definitionId, compactionContext);

            //RavenTestBase.WaitForUserToContinueTheTest(_documentStore);

            var loadedContext = await _queryStore.GetContext(definitionId);

            //RavenTestBase.WaitForUserToContinueTheTest(_documentStore);

            Assert.Equal(context.ToString(), loadedContext.ToString());
        }

        [Fact]
        public async Task CanStoreAndLoadHierarchy()
        {
            var root = new IndexableObjectProperty();
            var hierarchy = new ConstructQueryAccessibleProperties()
            {
                RootProperty = root
            };

            root.AddProperty("title", "http://linked.opendata.cz/ontology/drug-encyclopedia/title",
                new IndexableValueProperty() { Type = ValuePropertyType.LanguageString });
            var medicinalProductGroup = new IndexableObjectProperty();
            root.AddProperty("hasMedicinalProductGroup", "http://linked.opendata.cz/ontology/drug-encyclopedia/hasMedicinalProductGroup", medicinalProductGroup);
            medicinalProductGroup.AddProperty("title", "http://linked.opendata.cz/ontology/drug-encyclopedia/title", new IndexableValueProperty() { Type = ValuePropertyType.LanguageString });
            medicinalProductGroup.AddProperty("description", "http://linked.opendata.cz/ontology/drug-encyclopedia/description", new IndexableValueProperty() { Type = ValuePropertyType.Value });
            var atc = new IndexableObjectProperty();
            medicinalProductGroup.AddProperty("hasATCConcept", "http://linked.opendata.cz/ontology/drug-encyclopedia/hasATCConcept", atc);
            atc.AddProperty("title", "http://linked.opendata.cz/ontology/drug-encyclopedia/title", new IndexableValueProperty() { Type = ValuePropertyType.LanguageString });

            const string definitionId = "Query/1";

            await _queryStore.StoreHierarchy(definitionId, hierarchy);

            //RavenTestBase.WaitForUserToContinueTheTest(_documentStore);

            var loadedHierarchy = await _queryStore.GetHierarchy(definitionId);

            Assert.NotNull(loadedHierarchy);
        }

        [Fact]
        public async Task CanStoreNewIndex()
        {
            const string definitionId = "Query/1";

            const string indexId = definitionId + "/_id";

            var indexDefinition = new DragqnLDIndexDefiniton()
            {
                Name = indexId,
                RavenMap = @"from doc in docs
where doc[""@metadata""][""Raven-Entity-Name""] == ""Query/1""
select new { 
_id = doc._id,
_metadata_Raven_Entity_Name = doc[""@metadata""][""Raven-Entity-Name""]}",
                RavenAnalyzers = new Dictionary<string, string>() { { "_id", KnownRavenDBAnalyzers.AnalyzerLuceneStandard } }
            };

            await _queryStore.StoreIndex(definitionId, indexDefinition);

            RavenTestBase.WaitForUserToContinueTheTest(this._documentStore);
            var loadedIndex = await this._documentStore.AsyncDatabaseCommands.GetIndexAsync(indexDefinition.Name);
            Assert.NotNull(loadedIndex);

            var indexDefinitions = await _queryStore.GetIndexes(definitionId);
            Assert.True(indexDefinitions.Indexes.ContainsKey(indexId));
        }

        [Fact]
        public async Task CanRewriteCurrentIndex()
        {
            const string definitionId = "Query/1";

            const string indexId = definitionId + "/_id";
            var indexDefinition = new DragqnLDIndexDefiniton()
            {
                Name = indexId,
                RavenMap = @"from doc in docs
where doc[""@metadata""][""Raven-Entity-Name""] == ""Query/1""
select new { 
_id = doc._id,
_metadata_Raven_Entity_Name = doc[""@metadata""][""Raven-Entity-Name""]}",
                RavenAnalyzers = new Dictionary<string, string>() { { "_id", KnownRavenDBAnalyzers.AnalyzerLuceneStandard } }
            };

            await _queryStore.StoreIndex(definitionId, indexDefinition);

            var newIndexDefinition = new DragqnLDIndexDefiniton()
            {
                Name = indexId,
                RavenMap = @"from doc in docs
where doc[""@metadata""][""Raven-Entity-Name""] == ""Query/1""
select new { 
_id = doc._id,
_metadata_Raven_Entity_Name = doc[""@metadata""][""Raven-Entity-Name""]}",
                RavenAnalyzers = new Dictionary<string, string>()
            };
            await _queryStore.StoreIndex(definitionId, newIndexDefinition);

            //RavenTestBase.WaitForUserToContinueTheTest(this._documentStore);
            var loadedIndex = await this._documentStore.AsyncDatabaseCommands.GetIndexAsync(indexId);
            Assert.NotNull(loadedIndex);

            var indexDefinitions = await _queryStore.GetIndexes(definitionId);
            Assert.True(indexDefinitions.Indexes.ContainsKey(indexId));
            Assert.True(!indexDefinitions.Indexes[indexId].RavenAnalyzers.Any());
        }

        [Fact]
        public async Task CanAddMultipleIndexesToSameQueryDefinitoin()
        {
            const string definitionId = "Query/1";

            const string indexId = definitionId + "/_id";
            var indexDefinition = new DragqnLDIndexDefiniton()
            {
                Name = indexId,
                RavenMap = @"from doc in docs
where doc[""@metadata""][""Raven-Entity-Name""] == ""Query/1""
select new { 
_id = doc._id,
_metadata_Raven_Entity_Name = doc[""@metadata""][""Raven-Entity-Name""]}",
                RavenAnalyzers = new Dictionary<string, string>() { { "_id", KnownRavenDBAnalyzers.AnalyzerLuceneStandard } }
            };

            await _queryStore.StoreIndex(definitionId, indexDefinition);

            const string newIndexId = indexId + "_type";
            var newIndexDefinition = new DragqnLDIndexDefiniton()
            {
                Name = newIndexId,
                RavenMap = @"from doc in docs
where doc[""@metadata""][""Raven-Entity-Name""] == ""Query/1""
select new { 
_id = doc._id,
_type = doc._type,
_metadata_Raven_Entity_Name = doc[""@metadata""][""Raven-Entity-Name""]}",
                RavenAnalyzers = new Dictionary<string, string>()
            };
            await _queryStore.StoreIndex(definitionId, newIndexDefinition);

            //RavenTestBase.WaitForUserToContinueTheTest(this._documentStore);
            var loadedIndex = await this._documentStore.AsyncDatabaseCommands.GetIndexAsync(indexId);
            Assert.NotNull(loadedIndex);

            var indexDefinitions = await _queryStore.GetIndexes(definitionId);
            Assert.True(indexDefinitions.Indexes.ContainsKey(indexId));
            Assert.True(indexDefinitions.Indexes.ContainsKey(newIndexId));
        }

        [Fact]
        public async Task CanAddMultipleIndexesToDifferentQueryDefinitoin()
        {
            const string definitionId = "Query/3";

            const string indexId = definitionId + "/_id";
            var indexDefinition = new DragqnLDIndexDefiniton()
            {
                Name = indexId,
                RavenMap = @"from doc in docs
where doc[""@metadata""][""Raven-Entity-Name""] == ""Query/3""
select new { 
_id = doc._id,
_metadata_Raven_Entity_Name = doc[""@metadata""][""Raven-Entity-Name""]}",
                RavenAnalyzers = new Dictionary<string, string>() { { "_id", KnownRavenDBAnalyzers.AnalyzerLuceneStandard } }
            };

            await _queryStore.StoreIndex(definitionId, indexDefinition);

            const string definitionId2 = "Query/2";
            const string index2Id = definitionId2 + "/_id";
            var newIndexDefinition = new DragqnLDIndexDefiniton()
            {
                Name = index2Id,
                RavenMap = @"from doc in docs
where doc[""@metadata""][""Raven-Entity-Name""] == ""Query/2""
select new { 
_id = doc._id,
_metadata_Raven_Entity_Name = doc[""@metadata""][""Raven-Entity-Name""]}",
                RavenAnalyzers = new Dictionary<string, string>()
            };
            await _queryStore.StoreIndex(definitionId2, newIndexDefinition);

            //RavenTestBase.WaitForUserToContinueTheTest(this._documentStore);
            var loadedIndex = await this._documentStore.AsyncDatabaseCommands.GetIndexAsync(indexId);
            var loadedIndex2 = await _documentStore.AsyncDatabaseCommands.GetIndexAsync(index2Id);
            Assert.NotNull(loadedIndex);
            Assert.NotNull(loadedIndex2);

            var indexDefinitions = await _queryStore.GetIndexes(definitionId);
            Assert.True(indexDefinitions.Indexes.ContainsKey(indexId));
            Assert.Equal(1, indexDefinitions.Indexes.Count);
            var indexDefinitions2 = await _queryStore.GetIndexes(definitionId2);
            Assert.True(indexDefinitions2.Indexes.ContainsKey(index2Id));
            Assert.Equal(1, indexDefinitions2.Indexes.Count);
        }
    }
}
