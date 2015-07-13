using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Annotations;
using JsonLD.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raven.Json.Linq;

namespace DragqnLD.Core.Implementations
{
    [UsedImplicitly]
    public class DataLoader : IDataLoader
    {
        private readonly IQueryStore _queryStore;
        private readonly ISparqlEnpointClient _sparqlEnpointClient;
        private readonly IDataFormatter _dataFormatter;
        private readonly IDataStore _dataStore;
        private readonly IConstructAnalyzer _constructAnalyzer;

        public DataLoader(IQueryStore queryStore,
            ISparqlEnpointClient sparqlEnpointClient,
            IDataFormatter dataFormatter,
            IDataStore dataStore,
            IConstructAnalyzer constructAnalyzer)
        {
            _queryStore = queryStore;
            _sparqlEnpointClient = sparqlEnpointClient;
            _dataFormatter = dataFormatter;
            _dataStore = dataStore;
            _constructAnalyzer = constructAnalyzer;
        }

        //todo: according to first architecture, should also store the querydefinition
        public async Task Load(string definitionId, CancellationToken cancellationToken,
            IProgress<QueryDefinitionStatus> progress)
        {
            //todo: do some statistics
            // -- could be just in form 
            //   - total count - known from select result
            //   - current - ravendb count of stored docs + 1
            cancellationToken.ThrowIfCancellationRequested();

            var status = QueryDefinitionStatus.From(QueryStatus.LoadingSelectResult);
            if (progress != null)
            {
                progress.Report(status);
            }

            var qd = await _queryStore.Get(definitionId).ConfigureAwait(false);

            var selectResults = await _sparqlEnpointClient.QueryForUris(qd.SelectQuery).ConfigureAwait(false);
            var selectResultCount = selectResults.Count();

            status.Status = QueryStatus.LoadingDocuments;
            status.DocumentLoadProgress.TotalCount = selectResultCount;
            if (progress != null)
            {
                progress.Report(status);
            }

            var parsedSparqlQuery = ConstructAnalyzerHelper.ReplaceParamAndParseConstructQuery(qd);
            var compactionContext = _constructAnalyzer.CreateCompactionContextForQuery(parsedSparqlQuery);
            
            //Has to be stored and retrieved as ravenJObject, so lets convert in here for comapction purpuses to Context
            var compactionContextString = compactionContext.ToString();
            var parsed = JObject.Parse(compactionContextString);
            var convertedCompactionContext = new Context(parsed);
            convertedCompactionContext.Remove("@base");

            //todo: store any additional info? date produced etc.
            var contextId = await _queryStore.StoreContext(definitionId, compactionContext);

            //get property hiearchies and save them after getting their types
            var hierarchy = _constructAnalyzer.CreatePropertyPathsForQuery(parsedSparqlQuery, compactionContext);
            
            status.DocumentLoadProgress.CurrentItem = 0;
            PropertyMappings allMappings = new PropertyMappings();
            //done: start processing selects .. :)
            //todo: can be faster if getting batches of items and streaming to raven
            foreach (var selectResult in selectResults)
            {
                status.DocumentLoadProgress.CurrentItem++;
                if (progress != null)
                {
                    progress.Report(status);
                }

                cancellationToken.ThrowIfCancellationRequested();

                var constructResultStream = await _sparqlEnpointClient
                    .GetContructResultFor(qd.ConstructQuery, qd.ConstructQueryUriParameterName, selectResult)
                    .ConfigureAwait(false);

                var reader = new StreamReader(constructResultStream);

                var writer = new StringWriter();
                PropertyMappings mappings;
                //todo: discovering mappings here should be used only for Describe queries - Constructs should have static mappings
                //  - construct query mappings should be discovered from the query
                _dataFormatter.Format(reader, writer, selectResult.ToString(), convertedCompactionContext, out mappings);
                allMappings.Merge(mappings);
                var result = writer.ToString();

                var dataToStore = new ConstructResult
                {
                    QueryId = qd.Id,
                    DocumentId = selectResult,
                    Document = new Document {Content = RavenJObject.Parse(result)}
                };

                //done: store select results for viewing
                await _dataStore.StoreDocument(dataToStore).ConfigureAwait(false);

                //todo: document count could be a raven index - should it? :) 
            }

            //save hierachy after it has value types added
            var hierarchyId = await _queryStore.StoreHierarchy(definitionId, hierarchy);
            // todo: if this was one session, this wouldn't be necessary.. but then i'd rely on ravens tracking
            await _queryStore.StoreMappings(definitionId, allMappings);

            status.Status = QueryStatus.Loaded;
            if (progress != null)
            {
                progress.Report(status);
            }

            await _queryStore.UpdateLastRun(definitionId, DateTime.Now);
        }
    }
}
