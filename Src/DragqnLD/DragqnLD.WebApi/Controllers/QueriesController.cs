using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DragqnLD.Core.Abstraction.Query;

namespace DragqnLD.WebApi.Controllers
{
    public class QueriesController : ApiController
    {
        // GET api/queries
        public IEnumerable<QueryDefinition> Get()
        {
            return new[] 
            { 
                new QueryDefinition() 
                { 
                    Id = "Query/1", 
                    Name = "Test Query", 
                    Description = "Mockup Query Description" 
                },
                new QueryDefinition()
                {
                    Id = "Query/2",
                    Name = "Test Query 2",
                    Description = "Mockup Query Descirption 2"
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
                Description = "Test"
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
