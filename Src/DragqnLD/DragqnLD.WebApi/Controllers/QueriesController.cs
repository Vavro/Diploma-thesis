using System;
using System.Collections.Generic;
using System.Web.Http;
using AutoMapper;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.WebApi.Models;

namespace DragqnLD.WebApi.Controllers
{
    public class QueriesController : BaseApiController
    {
        // GET api/queries
        public IEnumerable<QueryDefinitionMetadataDto> Get()
        {
            var queries = new[]
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
            };
            var queriesDto = Mapper.Map<IEnumerable<QueryDefinition>, IEnumerable<QueryDefinitionMetadataDto>>(queries);
            return queriesDto;
        }

        // GET api/queries/5
        public QueryDefinitionDto Get(string id)
        {
            var query = new QueryDefinition
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

            var queryDto = Mapper.Map<QueryDefinition, QueryDefinitionDto>(query);
            return queryDto;
        }

        // POST api/queries
        public void Post([FromBody]QueryDefinitionDto value)
        {
            
        }

        // PUT api/queries/5
        public void Put(int id, [FromBody]QueryDefinitionDto value)
        {
        }

        // DELETE api/queries/5
        public void Delete(int id)
        {
        }
    }
}
