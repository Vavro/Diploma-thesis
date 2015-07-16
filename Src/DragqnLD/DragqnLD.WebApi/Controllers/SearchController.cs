using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI;
using DragqnLD.Core.Abstraction;
using DragqnLD.WebApi.Models;

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
        //todo: better name  - like searchRaw
        [Route("api/query/{definitionId}/searchEscaped")]
        public async Task<HttpResponseMessage> SearchEscapedLuceneQuery(string escapedQuery)
        {
            try
            {
                //todo: add paging
                var results = await _dataStore.QueryDocumentEscapedLuceneQuery(DefinitionId, escapedQuery);

                //todo: streamline
                var documentMetadataDtos =
                    results.Select(result => new DocumentMetadataDto() {Id = result.ToString()}).ToList();
                var pagedDocumentMetadataDto = new PagedDocumentMetadataDto()
                {
                    Items = documentMetadataDtos,
                    TotalItems = 1000
                };

                var response = CreateResponseWithObject(pagedDocumentMetadataDto);

                return response;
            }
            catch (Exception e)
            {
                Log.Fatal("exception", e);
                throw;
            }
        }

        /// <summary>
        /// Searchs the index by lucene query.
        /// </summary>
        /// <param name="indexId">The index identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/searchLucene/{*indexId}")]
        public async Task<HttpResponseMessage> SearchIndexByLuceneQuery(string indexId)
        {
            return CreateResponse(HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Searchs the dynamic index by lucene query.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/searchDynamicLucene")]
        public async Task<HttpResponseMessage> SearchDynamicIndexByLuceneQuery()
        {
            return CreateResponse(HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Searchs the index by sparql query.
        /// </summary>
        /// <param name="indexId">The index identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/searchSparql/{*indexId}")]
        public async Task<HttpResponseMessage> SearchIndexBySparqlQuery(string indexId)
        {
            return CreateResponse(HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Searchs the dynamic index by sparql.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/searchDynamicSparql")]
        public async Task<HttpResponseMessage> SearchDynamicIndexBySparql()
        {
            return CreateResponse(HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Searchs the index by lucene query.
        /// </summary>
        /// <param name="indexId">The index identifier.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/query/{definitionId}/searchLucene/{*indexId}")]
        public async Task<HttpResponseMessage> SearchIndexByLuceneQuery(string indexId, [FromBody] string query)
        {
            return CreateResponse(HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Searchs the dynamic index by lucene query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/query/{definitionId}/searchDynamicLucene")]
        public async Task<HttpResponseMessage> SearchDynamicIndexByLuceneQuery([FromBody] string query)
        {
            return CreateResponse(HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Searchs the index by sparql query.
        /// </summary>
        /// <param name="indexId">The index identifier.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/query/{definitionId}/searchSparql/{*indexId}")]
        public async Task<HttpResponseMessage> SearchIndexBySparqlQuery(string indexId, [FromBody] string query)
        {
            return CreateResponse(HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Searchs the dynamic index by sparql.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/query/{definitionId}/searchDynamicSparql")]
        public async Task<HttpResponseMessage> SearchDynamicIndexBySparql([FromBody] string query)
        {
            return CreateResponse(HttpStatusCode.NotImplemented);
        }
    }
}