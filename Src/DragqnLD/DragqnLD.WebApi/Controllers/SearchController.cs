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
        private readonly ISelectAnalyzer _selectAnalyzer;
        private readonly IQueryStore _queryStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController" /> class.
        /// </summary>
        /// <param name="dataStore">The data store.</param>
        /// <param name="selectAnalyzer">The select analyzer.</param>
        /// <param name="queryStore">The query store.</param>
        public SearchController(IDataStore dataStore, ISelectAnalyzer selectAnalyzer, IQueryStore queryStore)
        {
            _dataStore = dataStore;
            _selectAnalyzer = selectAnalyzer;
            _queryStore = queryStore;
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
        /// <param name="indexName">The index identifier.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/searchLucene/{*indexName}")]
        public async Task<HttpResponseMessage> SearchIndexByLuceneQuery(string indexName, string query)
        {
            //todo: add paging
            var results = await _dataStore.QueryDocumentEscapedLuceneQuery(DefinitionId, indexName, query);

            var documentMetadataDtos =
                results.Select(result => new DocumentMetadataDto() { Id = result.ToString() }).ToList();
            var pagedDocumentMetadataDto = new PagedDocumentMetadataDto()
            {
                Items = documentMetadataDtos,
                TotalItems = 1000
            };

            var response = CreateResponseWithObject(pagedDocumentMetadataDto);

            return response;
        }

        /// <summary>
        /// Searchs the dynamic index by lucene query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/searchDynamicLucene")]
        public async Task<HttpResponseMessage> SearchDynamicIndexByLuceneQuery(string query)
        {
            //todo: add paging
            var results = await _dataStore.QueryDocumentEscapedLuceneQuery(DefinitionId, query);

            var documentMetadataDtos =
                results.Select(result => new DocumentMetadataDto() { Id = result.ToString() }).ToList();
            var pagedDocumentMetadataDto = new PagedDocumentMetadataDto()
            {
                Items = documentMetadataDtos,
                TotalItems = 1000
            };

            var response = CreateResponseWithObject(pagedDocumentMetadataDto);

            return response;
        }

        /// <summary>
        /// Searchs the index by sparql query.
        /// </summary>
        /// <param name="indexName">The index identifier.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/searchSparql/{*indexName}")]
        public async Task<HttpResponseMessage> SearchIndexBySparqlQuery(string indexName, string query)
        {
            var index = await _queryStore.GetIndex(this.DefinitionId, indexName);
            var hierarchy = await _queryStore.GetHierarchy(this.DefinitionId);
            var luceneQuery = _selectAnalyzer.ConvertSparqlToLuceneWithIndex(query, hierarchy, index);

            return await SearchIndexByLuceneQuery(indexName, luceneQuery);
        }

        /// <summary>
        /// Searchs the dynamic index by sparql.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/searchDynamicSparql")]
        public async Task<HttpResponseMessage> SearchDynamicIndexBySparql(string query)
        {
            var hierarchy = await _queryStore.GetHierarchy(this.DefinitionId);
            var luceneQuery = _selectAnalyzer.ConvertSparqlToLuceneNoIndex(query, hierarchy);

            return await SearchDynamicIndexByLuceneQuery(luceneQuery);
        }

        /// <summary>
        /// Searchs the index by lucene query.
        /// </summary>
        /// <param name="indexName">The index identifier.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/query/{definitionId}/searchLucene/{*indexName}")]
        public async Task<HttpResponseMessage> SearchIndexByLuceneQueryPost(string indexName, [FromBody] string query)
        {
            return await SearchIndexByLuceneQuery(indexName, query);
        }

        /// <summary>
        /// Searchs the dynamic index by lucene query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/query/{definitionId}/searchDynamicLucene")]
        public async Task<HttpResponseMessage> SearchDynamicIndexByLuceneQueryPost([FromBody] string query)
        {
            return await SearchDynamicIndexByLuceneQuery(query);
        }

        /// <summary>
        /// Searchs the index by sparql query.
        /// </summary>
        /// <param name="indexName">The index identifier.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/query/{definitionId}/searchSparql/{*indexName}")]
        public async Task<HttpResponseMessage> SearchIndexBySparqlQueryPost(string indexName, [FromBody] string query)
        {
            return await SearchIndexBySparqlQuery(indexName, query);
        }

        /// <summary>
        /// Searchs the dynamic index by sparql.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/query/{definitionId}/searchDynamicSparql")]
        public async Task<HttpResponseMessage> SearchDynamicIndexBySparqlPost([FromBody] string query)
        {
            return await SearchDynamicIndexBySparql(query);
        }
    }
}