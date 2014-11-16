using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Implementations;
using DragqnLD.WebApi.Models;
using Newtonsoft.Json.Linq;
using Raven.Imports.Newtonsoft.Json;
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

        /// <summary>
        /// Processes the specified definition identifier.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/process")]
        public async Task<HttpResponseMessage> Process(string definitionId)
        {
            //todo: make this a background task pushed into backgroundworker
            HostingEnvironment.QueueBackgroundWorkItem(async ct => 
            {
                //todo: store cancellation token by definitionId so that task can get cancelled
                //todo: do some statistics
                // -- could be just in form 
                //   - total count - known from select result
                //   - current - ravendb count of stored docs + 1
                
                //todo: move this to the Core assembly
                var qd = await _queryStore.Get(definitionId);

                var selectResults = await _sparqlEnpointClient.QueryForUris(qd.SelectQuery);

                //done: start processing selects .. :)
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

                    //done: store select results for viewing
                    await _dataStore.StoreDocument(dataToStore);

                    //todo: statistics
                    //todo: update document count ? -- maybe should be an raven index :) 
                }
            });
            
            return CreateResponse();
        }

        /// <summary>
        /// Cancels the processing of the query definition.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/process/cancel")]
        public async Task<HttpResponseMessage> Cancel(string definitionId)
        {
            //todo: get cancelationToken for definitionId and try to cancel

            //todo: response according to cancel
            return CreateResponse();
        }

        /// <summary>
        /// Retuns only status of th especified definition.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        /// <returns>A <see cref="QueryDefinitionStatusDto"/></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/process/status")]
        public async Task<HttpResponseMessage> Status(string definitionId)
        {
            var status = new QueryDefinitionStatusDto()
            {
                DocumentLoadProgress = new ProgressDto() {CurrentItem = 16, TotalCount = 12345},
                Status = QueryStatus.LoadingDocuments
            };

            var response = CreateResponseWithObject(status);

            return response;
        }
    }
}
