using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;
using DragqnLD.WebApi.Models;

namespace DragqnLD.WebApi.Controllers
{
    public class QueriesController : BaseApiController
    {
        private readonly IQueryStore _queryStore;

        public QueriesController()
        {
            _queryStore = new QueryStore(Store);
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
            var taskManager = new TaskManager();
            //todo: reconsider doing this async/sync
            var statistics = await taskManager.GetStatusOfQuery(definitionId);

            var queryDto = Mapper.Map<QueryDefinition, QueryDefinitionWithStatusDto>(queryDefinition);
            queryDto.Status = new QueryDefinitionStatusDto()
            {
                DocumentLoadProgress = new ProgressDto()
                {
                    CurrentItem = statistics.DocumentLoadProgress.CurrentItem,
                    TotalCount = statistics.DocumentLoadProgress.TotalCount
                },
                Status = statistics.Status
            };
            queryDto.StoredDocumentCount = 1234;
            return queryDto;
        }

        // POST api/queries
        /// <summary>
        /// Stores a new query definition.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Post([FromBody]QueryDefinitionDto value)
        {
            var success = await StoreQueryDefinition(value);

            //todo: change reponse according to success
            //done: response 200
            var response = CreateResponse();

            return response;
        }

        private async Task<HttpResponseMessage> StoreQueryDefinition(QueryDefinitionDto value)
        {
            //todo: add exception logging and maybe routing to clinet
            var queryDefinition = Mapper.Map<QueryDefinitionDto, QueryDefinition>(value);

            await _queryStore.Add(queryDefinition);

            //todo: return id of created query definition
            return CreateResponse();
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
            var success = await StoreQueryDefinition(queryDefinition);

            //todo: use success state value
            return CreateResponse();
        }

        // DELETE api/queries/5
        public HttpResponseMessage Delete(int id)
        {
            return CreateResponse(HttpStatusCode.NotImplemented);
        }
    }

    /// <summary>
    /// Mockup how it could be
    /// </summary>
    //todo: implement! :D
    public class TaskManager
    {
        public Task<QueryDefinitionStatus> GetStatusOfQuery(string id)
        {
            return
                Task<QueryDefinitionStatus>.Factory.StartNew(
                    () =>
                        new QueryDefinitionStatus()
                        {
                            Status = QueryStatus.LoadingDocuments,
                            DocumentLoadProgress = new Progress() { CurrentItem = 15, TotalCount = 1234 }
                        });
        }
    }

    public class QueryDefinitionStatus
    {
        public QueryStatus Status { get; set; }
        public Progress DocumentLoadProgress { get; set; }
    }

    public class Progress
    {
        public int CurrentItem { get; set; }
        public int TotalCount { get; set; }
    }
}
