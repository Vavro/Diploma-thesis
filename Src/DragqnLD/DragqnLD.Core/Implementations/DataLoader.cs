using System.IO;
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

        public async Task Load(CancellationToken cancellationToken, string definitionId)
        {
            //todo: do some statistics
            // -- could be just in form 
            //   - total count - known from select result
            //   - current - ravendb count of stored docs + 1
            cancellationToken.ThrowIfCancellationRequested();

            var qd = await _queryStore.Get(definitionId).ConfigureAwait(false);
            
            var selectResults = await _sparqlEnpointClient.QueryForUris(qd.SelectQuery).ConfigureAwait(false);

            //done: start processing selects .. :)
            foreach (var selectResult in selectResults)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var constructResultStream = await _sparqlEnpointClient
                    .GetContructResultFor(qd.ConstructQuery, qd.ConstructQueryUriParameterName, selectResult)
                    .ConfigureAwait(false);

                var reader = new StreamReader(constructResultStream);

                var writer = new StringWriter();
                PropertyMappings mappings;
                _dataFormatter.Format(reader, writer, selectResult.ToString(), out mappings);

                var result = writer.ToString();

                var dataToStore = new ConstructResult
                {
                    QueryId = qd.Id,
                    DocumentId = selectResult,
                    Document = new Document { Content = RavenJObject.Parse(result) }
                };

                //done: store select results for viewing
                await _dataStore.StoreDocument(dataToStore).ConfigureAwait(false);

                //todo: statistics
                //todo: update document count ? -- maybe should be an raven index :) 
            }
        
        }
    }
}
