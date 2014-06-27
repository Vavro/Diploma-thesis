using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
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
                new QueryDefinition()
                {
                    Id = "Query/1",
                    Name = "Test Query",
                    Description = "Mockup Query Description",
                    ConstructQuery = new SparqlQueryInfo()
                    {
                        //parameters that can be substituted have to be marked as @ not just ?
                        Query = @"DESCRIBE @u",
                        DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                        SparqlEndpoint = new Uri(@"http://linked.opendata.cz/sparql")
                    },
                    ConstructQueryUriParameterName = "u",
                    SelectQuery = new SparqlQueryInfo()
                    {
                        Query = @"SELECT DISTINCT ?s WHERE { ?s ?p ?o } LIMIT 100",
                        DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                        SparqlEndpoint = new Uri(@"http://linked.opendata.cz/sparql")
                    }
                },
                new QueryDefinition()
                {
                    Id = "Query/2",
                    Name = "Test Query 2",
                    Description = "Mockup Query Descirption 2",
                    ConstructQuery = new SparqlQueryInfo()
                    {
                        //parameters that can be substituted have to be marked as @ not just ?
                        Query = @"DESCRIBE @u",
                        DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                        SparqlEndpoint = new Uri(@"http://linked.opendata.cz/sparql")
                    },
                    ConstructQueryUriParameterName = "u",
                    SelectQuery = new SparqlQueryInfo()
                    {
                        Query = @"SELECT DISTINCT ?s WHERE { ?s ?p ?o } LIMIT 100",
                        DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                        SparqlEndpoint = new Uri(@"http://linked.opendata.cz/sparql")
                    }
                }
            };


        }

        // GET api/queries/5
        public QueryDefinition Get(string id)
        {
            return new QueryDefinition()
            {
                Id = id,
                Name = "Test",
                Description = "Test",
                ConstructQuery = new SparqlQueryInfo()
                {
                    //parameters that can be substituted have to be marked as @ not just ?
                    Query = @"DESCRIBE @u",
                    DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                    SparqlEndpoint = new Uri(@"http://linked.opendata.cz/sparql")
                },
                ConstructQueryUriParameterName = "u",
                SelectQuery = new SparqlQueryInfo()
                {
                    Query = @"SELECT DISTINCT ?s WHERE { ?s ?p ?o } LIMIT 100",
                    DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                    SparqlEndpoint = new Uri(@"http://linked.opendata.cz/sparql")
                }
            };
        }

        // POST api/queries
        public void Post([FromBody]QueryDefinition value)
        {
            
        }

        // PUT api/queries/5
        public void Put(int id, [FromBody]QueryDefinition value)
        {
        }

        // DELETE api/queries/5
        public void Delete(int id)
        {
        }
    }
}
