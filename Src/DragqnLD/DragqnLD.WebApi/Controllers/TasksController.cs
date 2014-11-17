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
using DragqnLD.Core.Abstraction.Query;
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
        private readonly PerQueryDefinitionTasksManager _queryDefinitionLoadTasksManager;
        private DataLoader _dataLoader;

        public TasksController()
        {
            //todo: injection!!
            _dataStore = new RavenDataStore(Store, new DocumentPropertyEscaper());
            _queryStore = new QueryStore(Store);
            _sparqlEnpointClient = new SparqlEnpointClient();
            _dataFormatter = new ExpandedJsonLDDataFormatter();
            _queryDefinitionLoadTasksManager = PerQueryDefinitionTasksManager.Instance;
            _dataLoader = new DataLoader(_queryStore, _sparqlEnpointClient, _dataFormatter, _dataStore);
        }

        /// <summary>
        /// Processes the specified definition identifier.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        /// <returns>Http Status Code Accepted as the Processing is scheduled for background.</returns>
        [HttpGet]
        [Route("api/query/{definitionId}/process")]
        public HttpResponseMessage Process()
        {
            HostingEnvironment.QueueBackgroundWorkItem(ct =>
                _queryDefinitionLoadTasksManager.EnqueueTask(DefinitionId, ct,
                    (definitionId, cancellationToken, progress) =>
                        _dataLoader.Load(definitionId, cancellationToken, progress)));

            return CreateResponse(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// Cancels the processing of the query definition.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        /// <returns>Http Status Code Accepted as it tries to cancel the processing.</returns>
        [HttpGet]
        [Route("api/query/{definitionId}/process/cancel")]
        public HttpResponseMessage Cancel()
        {
            _queryDefinitionLoadTasksManager.TryCancel(DefinitionId);

            return CreateResponse(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// Retuns only status of th especified definition.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        /// <returns>A <see cref="QueryDefinitionStatusDto"/></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/process/status")]
        public async Task<HttpResponseMessage> Status()
        {
            var status = await _queryDefinitionLoadTasksManager.GetStatusOfQuery(DefinitionId);
            
            var response = CreateResponseWithObject(status);

            return response;
        }
    }
}
