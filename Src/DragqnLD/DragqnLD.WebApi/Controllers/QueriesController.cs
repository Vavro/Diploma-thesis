using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using AutoMapper;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.WebApi.Models;

namespace DragqnLD.WebApi.Controllers
{
    /// <summary>
    /// Basic operations for Query Definitions - store, load, delete, update
    /// </summary>
    public class QueriesController : BaseApiController
    {
        private readonly IQueryStore _queryStore;
        private readonly PerQueryDefinitionTasksManager _perQueryDefinitionTasksManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueriesController"/> class.
        /// </summary>
        /// <param name="queryStore">The query store.</param>
        /// <param name="perQueryDefinitionTasksManager">The per query definition tasks manager.</param>
        public QueriesController(IQueryStore queryStore, PerQueryDefinitionTasksManager perQueryDefinitionTasksManager)
        {
            _queryStore = queryStore;
            _perQueryDefinitionTasksManager = perQueryDefinitionTasksManager;
        }

        // GET api/queries
        /// <summary>
        /// Gets all stored query definitions.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<QueryDefinitionMetadataDto>> Get()
        {
            var queries = await _queryStore.GetAllDefinitions();

#if DEBUG
            if (!queries.Any())
            {
                queries = queries.Concat(
                    new[]
                    {
                        new QueryDefinition
                        {
                            Id = "Query/1",
                            Name = "Test Query",
                            Description = "Mockup Query Description",
                            ConstructQuery = new SparqlQueryInfo
                            {
                                //parameters that can be substituted have to be marked as @ not just ?
                                Query = @"DESCRIBE @u",
                                DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                                SparqlEndpoint = new Uri(@"http://linked.opendata.cz/sparql")
                            },
                            ConstructQueryUriParameterName = "u",
                            SelectQuery = new SparqlQueryInfo
                            {
                                Query = @"SELECT DISTINCT ?s WHERE { ?s ?p ?o } LIMIT 100",
                                DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                                SparqlEndpoint = new Uri(@"http://linked.opendata.cz/sparql")
                            }
                        },
                        new QueryDefinition
                        {
                            Id = "Query/2",
                            Name = "Test Query 2",
                            Description = "Mockup Query Descirption 2",
                            ConstructQuery = new SparqlQueryInfo
                            {
                                //parameters that can be substituted have to be marked as @ not just ?
                                Query = @"DESCRIBE @u",
                                DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                                SparqlEndpoint = new Uri(@"http://linked.opendata.cz/sparql")
                            },
                            ConstructQueryUriParameterName = "u",
                            SelectQuery = new SparqlQueryInfo
                            {
                                Query = @"SELECT DISTINCT ?s WHERE { ?s ?p ?o } LIMIT 100",
                                DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                                SparqlEndpoint = new Uri(@"http://linked.opendata.cz/sparql")
                            }
                        }
                    });
            }
#endif
            var queriesDto = Mapper.Map<IEnumerable<QueryDefinition>, IEnumerable<QueryDefinitionMetadataDto>>(queries);
            return queriesDto;
        }

        // GET api/queries/5
        /// <summary>
        /// Gets the query definition for the specified definition identifier.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        /// <returns></returns>
        public async Task<QueryDefinitionWithStatusDto> Get(string definitionId)
        {
            var queryDefinition = await _queryStore.Get(definitionId);
#if DEBUG
            if (queryDefinition == null)
            {
                queryDefinition = new QueryDefinition
                {
                    Id = definitionId,
                    Name = "Test",
                    Description = "Test",
                    ConstructQuery = new SparqlQueryInfo
                    {
                        //parameters that can be substituted have to be marked as @ not just ?
                        Query = @"DESCRIBE @u",
                        DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                        SparqlEndpoint = new Uri(@"http://linked.opendata.cz/sparql")
                    },
                    ConstructQueryUriParameterName = "u",
                    SelectQuery = new SparqlQueryInfo
                    {
                        Query = @"SELECT DISTINCT ?s WHERE { ?s ?p ?o } LIMIT 100",
                        DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                        SparqlEndpoint = new Uri(@"http://linked.opendata.cz/sparql")
                    }
                };
            }
#endif
            //todo: reconsider doing this async/sync
            var queryDto = Mapper.Map<QueryDefinition, QueryDefinitionWithStatusDto>(queryDefinition);

            var statistics = await _perQueryDefinitionTasksManager.GetStatusOfQuery(definitionId);
            if (statistics == null)
            {
                statistics = queryDefinition.GetStatus();
            }

            var statusDto = Mapper.Map<QueryDefinitionStatusDto>(statistics);
            queryDto.Status = statusDto;

            queryDto.StoredDocumentCount = await _queryStore.GetDocumentCount(DefinitionId);

            return queryDto;
        }

        // POST api/queries
        /// <summary>
        /// Stores a new query definition.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Post([FromBody]QueryDefinitionDto value)
        {
            var response = await StoreQueryDefinition(value);

            return response;
        }

        private async Task<HttpResponseMessage> StoreQueryDefinition(QueryDefinitionDto value)
        {
            //todo: add exception logging and maybe routing to clinet
            var queryDefinition = Mapper.Map<QueryDefinitionDto, QueryDefinition>(value);

            await _queryStore.Add(queryDefinition);

            //todo: return whether updated or created, if from POST only create is allowed?
            //done: return id of created query definition
            var response = CreateResponseWithObject(queryDefinition.Id, HttpStatusCode.Created);

            return response;
        }

        // PUT api/queries/5
        /// <summary>
        /// Stores or updates the query definition with the specified id
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="queryDefinition">The query definition.</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Put(int id, [FromBody]QueryDefinitionDto queryDefinition)
        {
            //todo: use the id (maybe delete from value)
            var response = await StoreQueryDefinition(queryDefinition);

            return response;
        }
        
        // DELETE api/queries/5
        /// <summary>
        /// Deletes the query definition with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public HttpResponseMessage Delete(int id)
        {
            return CreateResponse(HttpStatusCode.NotImplemented);
        }
    }

    /// <summary>
    /// Basic tasks overview, to have access to running tasks and their cancellation tokens
    /// </summary>
    public class PerQueryDefinitionTasksManager
    {
        private static object _instanceLock = new object();
        private static PerQueryDefinitionTasksManager _instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static PerQueryDefinitionTasksManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new PerQueryDefinitionTasksManager();
                        }
                    }
                }

                return _instance;
            }
        }

        private PerQueryDefinitionTasksManager()
        {

        }

        private class RunnigTaskData : IDisposable
        {
            //todo: not sure about thread safety 
            public CancellationTokenSource CancellationTokenSource { get; set; }
            public QueryDefinitionStatus LastStatus { get; set; }
            public void Dispose()
            {
                var cts = CancellationTokenSource;
                CancellationTokenSource = null;
                cts.Dispose();
            }
        }

        private ConcurrentDictionary<string, RunnigTaskData> _runnignTasks = new ConcurrentDictionary<string, RunnigTaskData>();

        /// <summary>
        /// Gets the status of query.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        /// <returns></returns>
        public Task<QueryDefinitionStatus> GetStatusOfQuery(string definitionId)
        {
            //todo: prepared for update to querying db - so still a task although not necessary
            return
                Task<QueryDefinitionStatus>.Factory.StartNew(
                    () =>
                    {
                        RunnigTaskData runnigTaskData;
                        if (_runnignTasks.TryGetValue(definitionId, out runnigTaskData))
                        {
                            return runnigTaskData.LastStatus;
                        }

                        return null;
                    });
        }

        /// <summary>
        /// Enqueues the task.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="loadQueryDefinitionTask">The load query definition task.</param>
        /// <returns></returns>
        public Task EnqueueTask(
            string definitionId,
            CancellationToken cancellationToken,
            Func<string, CancellationToken, Progress<QueryDefinitionStatus>, Task> loadQueryDefinitionTask)
        {
            var newCts = new CancellationTokenSource();
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, newCts.Token);

            var localDefinitionId = definitionId;
            var taskData = new RunnigTaskData() { CancellationTokenSource = newCts };
            _runnignTasks.TryAdd(localDefinitionId, taskData);
            var progress = new Progress<QueryDefinitionStatus>(status => UpdateStatus(definitionId, status));

            var task = loadQueryDefinitionTask(localDefinitionId, linkedCts.Token, progress).ContinueWith(_ =>
            {
                linkedCts.Dispose();
                RunnigTaskData taskDataForRemove;
                if (_runnignTasks.TryRemove(localDefinitionId, out taskDataForRemove))
                {
                    taskDataForRemove.Dispose();
                }
                //todo: maybe save somewhere done status for querydefinition
            }, cancellationToken);

            return task;
        }

        private void UpdateStatus(string definitionId, QueryDefinitionStatus status)
        {
            RunnigTaskData taskDataForProgress;
            _runnignTasks.TryGetValue(definitionId, out taskDataForProgress);

            Debug.Assert(taskDataForProgress != null, "taskDataForProgress != null");
            taskDataForProgress.LastStatus = status;
            //todo: could do live notification over a channel (websocket or event-stream)
        }

        /// <summary>
        /// Tries to cancel a running task.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        public void TryCancel(string definitionId)
        {
            RunnigTaskData runnigTaskData;
            var succ = _runnignTasks.TryGetValue(definitionId, out runnigTaskData);
            if (succ)
            {
                runnigTaskData.CancellationTokenSource.Cancel();
            }
        }
    }
}
