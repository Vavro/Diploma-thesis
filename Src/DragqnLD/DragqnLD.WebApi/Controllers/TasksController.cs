using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Implementations;
using Raven.Json.Linq;

namespace DragqnLD.WebApi.Controllers
{
    public class TasksController : BaseApiController
    {
        private IDataStore _dataStore;
        private IQueryStore _queryStore;
        private ISparqlEnpointClient _sparqlEnpointClient;
        private IDataFormatter _dataFormatter;
        public TasksController()
        {
            _dataStore = new RavenDataStore(Store, new DocumentPropertyEscaper());
            _queryStore = new QueryStore(Store);
            _sparqlEnpointClient = new SparqlEnpointClient();
            _dataFormatter = new ExpandedJsonLDDataFormatter();
        }

        public async Task<IEnumerable<Uri>>  Process(string id)
        {
            var qd = await _queryStore.Get(id);

            var selectResults = await _sparqlEnpointClient.QueryForUris(qd.SelectQuery);

            //todo: store select results for viewing
            //todo: start processing selects .. :)
            foreach (var selectResult in selectResults)
            {
                var constructResultStream = await _sparqlEnpointClient.GetContructResultFor(qd.ConstructQuery, qd.ConstructQueryUriParameterName, selectResult);
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
                
                await _dataStore.StoreDocument(dataToStore);

                //todo: statistics
                //todo: update document count ? -- maybe should be an raven index :) 
            }

            return selectResults;
        }
    }
}
