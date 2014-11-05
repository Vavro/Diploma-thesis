using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Implementations;

namespace DragqnLD.WebApi.Controllers
{
    public class TasksController : BaseApiController
    {
        private IDataStore _dataStore;
        private IQueryStore _queryStore;
        private ISparqlEnpointClient _sparqlEnpointClient;

        public TasksController()
        {
            _dataStore = new RavenDataStore(Store, new DocumentPropertyEscaper());
            _queryStore = new QueryStore(Store);
            _sparqlEnpointClient = new SparqlEnpointClient();
        }

        public async Task<IEnumerable<Uri>>  Get(string id)
        {
            var qd = await _queryStore.Get(id);

            var selectResults = await _sparqlEnpointClient.QueryForUris(qd.SelectQuery);

            //todo: store select results for viewing
            //todo: start processing selects .. :)


            return selectResults;
        }
    }
}
