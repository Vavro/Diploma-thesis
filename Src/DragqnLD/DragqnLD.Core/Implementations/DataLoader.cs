using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Abstraction.Query;
using Raven.Json.Linq;

namespace DragqnLD.Core.Implementations
{
    public class DataLoader : IDataLoader
    {
        private readonly IQueryStore _queryStore;
        private readonly ISparqlEnpointClient _sparqlEnpointClient;
        private readonly IDataFormatter _dataFormatter;
        private readonly IDataStore _dataStore;

        public DataLoader(IQueryStore queryStore, ISparqlEnpointClient sparqlEnpointClient, IDataFormatter dataFormatter, IDataStore dataStore)
        {
            _queryStore = queryStore;
            _sparqlEnpointClient = sparqlEnpointClient;
            _dataFormatter = dataFormatter;
            _dataStore = dataStore;
        }

        //todo: according to first architecture, should also store the querydefinition
        public async Task Load(string definitionId, CancellationToken cancellationToken, IProgress<QueryDefinitionStatus> progress)
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
                _dataFormatter.Format(reader, writer, selectResult.ToString(), out mappings);
                allMappings.Merge(mappings);
                var result = writer.ToString();

                var dataToStore = new ConstructResult
                {
                    QueryId = qd.Id,
                    DocumentId = selectResult,
                    Document = new Document { Content = RavenJObject.Parse(result) }
                };

                //done: store select results for viewing
                await _dataStore.StoreDocument(dataToStore).ConfigureAwait(false);

                //todo: document count could be a raven index - should it? :) 
            }

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
