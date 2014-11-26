using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using AutoMapper;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;
using DragqnLD.WebApi.Models;

namespace DragqnLD.WebApi.Controllers
{
    /// <summary>
    /// Manages backgrounds tasks availible for QueryDefinitions
    /// </summary>
    public class TasksController : BaseApiController
    {
        private readonly IQueryStore _queryStore;
        private readonly PerQueryDefinitionTasksManager _queryDefinitionLoadTasksManager;
        private readonly DataLoader _dataLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="TasksController"/> class.
        /// </summary>
        /// <param name="queryStore">The query store.</param>
        /// <param name="queryDefinitionLoadTasksManager">The query definition load tasks manager.</param>
        /// <param name="dataLoader">The data loader.</param>
        public TasksController(IQueryStore queryStore, PerQueryDefinitionTasksManager queryDefinitionLoadTasksManager, DataLoader dataLoader)
        {
            _queryStore = queryStore;
            _queryDefinitionLoadTasksManager = queryDefinitionLoadTasksManager;
            _dataLoader = dataLoader;
        }

        /// <summary>
        /// Processes the specified definition identifier.
        /// </summary>
        /// <returns>Http Status Code Accepted as the Processing is scheduled for background.</returns>
        [HttpGet]
        [Route("api/query/{definitionId}/process")]
        public HttpResponseMessage Process()
        {
            //todo: ensure two same def id cant be processed at one time
            HostingEnvironment.QueueBackgroundWorkItem(ct =>
                _queryDefinitionLoadTasksManager.EnqueueTask(DefinitionId, ct,
                    (definitionId, cancellationToken, progress) =>
                        _dataLoader.Load(definitionId, cancellationToken, progress)));

            return CreateResponse(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// Cancels the processing of the query definition.
        /// </summary>
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
        /// <returns>A <see cref="QueryDefinitionStatusDto"/></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/process/status")]
        public async Task<HttpResponseMessage> Status()
        {
            var status = await _queryDefinitionLoadTasksManager.GetStatusOfQuery(DefinitionId);
            if (status == null)
            {
                var qd = await _queryStore.Get(DefinitionId);
                status = qd.GetStatus();
            }
            var statusDto = Mapper.Map<QueryDefinitionStatusDto>(status);
            var response = CreateResponseWithObject(statusDto);

            return response;
        }
    }
}
