using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DragqnLD.Core.Abstraction;

namespace DragqnLD.WebApi.Controllers
{
    /// <summary>
    /// Returns contexts for 
    /// </summary>
    public class ContextsController : BaseApiController
    {
        private readonly IQueryStore _queryStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextsController"/> class.
        /// </summary>
        /// <param name="queryStore">The query store.</param>
        public ContextsController(IQueryStore queryStore)
        {
            _queryStore = queryStore;
        }
        /// <summary>
        /// Gets the context for the specified definition identifier.
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Get()
        {
            var context = await _queryStore.GetContext(DefinitionId);

            var response = CreateJsonResponse(context);
            return response;
        }

    }
}
