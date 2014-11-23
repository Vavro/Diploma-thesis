using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DragqnLD.Core.Abstraction;

namespace DragqnLD.WebApi.Controllers
{
    /// <summary>
    /// Controller that provides searching API
    /// </summary>
    public class SearchController : BaseApiController
    {
        private readonly IDataStore _dataStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController"/> class.
        /// </summary>
        /// <param name="dataStore">The data store.</param>
        public SearchController(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        /// <summary>
        /// Searchs by the escaped lucene query.
        /// </summary>
        /// <param name="escapedQuery">The escapted query.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/searchEscaped")]
        public async Task<HttpResponseMessage> SearchEscapedLuceneQuery(string escapedQuery)
        {
            var results = await _dataStore.QueryDocumentEscapedLuceneQuery(DefinitionId, escapedQuery);

            //todo: streamline
            var transformedResults = results.Select(result => new {Id = result.ToString()});

            var response = CreateResponseWithObject(transformedResults);

            return response;
        }
    }
}