using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<QueryDefinitionWithStatusDto> Get(string id)
        {
            var queryDefinition = await _queryStore.Get(id);
#if DEBUG
            if (queryDefinition == null)
            {
                queryDefinition = new QueryDefinition
                {
                    Id = id,
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
            var statistics = await taskManager.GetStatusOfQuery(id);

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
        public async Task Post([FromBody]QueryDefinitionDto value)
        {
            await StoreQueryDefinition(value);

            //todo: response 200
        }

        private async Task StoreQueryDefinition(QueryDefinitionDto value)
        {
            //todo: add exception logging and maybe routing to clinet
            var queryDefinition = Mapper.Map<QueryDefinitionDto, QueryDefinition>(value);

            await _queryStore.Add(queryDefinition);
        }

        // PUT api/queries/5
        public async Task Put(int id, [FromBody]QueryDefinitionDto value)
        {
            await StoreQueryDefinition(value);
        }

        // DELETE api/queries/5
        public void Delete(int id)
        {
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
