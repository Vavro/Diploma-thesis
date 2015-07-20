using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Indexes;
using DragqnLD.WebApi.Models;

namespace DragqnLD.WebApi.Controllers
{

    /// <summary>
    /// Translates from sparql to lucene queries
    /// </summary>
    public class TranslatorController : BaseApiController
    {
        private readonly IQueryStore _queryStore;
        private readonly ISelectAnalyzer _selectAnalyzer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatorController" /> class.
        /// </summary>
        /// <param name="queryStore">The query store.</param>
        /// <param name="selectAnalyzer">The select analyzer.</param>
        public TranslatorController(IQueryStore queryStore, ISelectAnalyzer selectAnalyzer)
        {
            _queryStore = queryStore;
            _selectAnalyzer = selectAnalyzer;
        }


        /// <summary>
        /// Searchs by the escaped lucene query.
        /// </summary>
        /// <param name="sparqlQuery">The escapted query.</param>
        /// <returns></returns>
        [HttpGet]
        //todo: better name  - like searchRaw
        [Route("api/query/{definitionId}/translate")]
        [Route("api/query/{definitionId}/translate/{*indexName}")]
        public async Task<HttpResponseMessage> TranslateSparqlToLuceneQuery(string sparqlQuery, string indexName = null)
        {
            var hierarchy = await _queryStore.GetHierarchy(this.DefinitionId);
            string luceneQuery;
            if (indexName != null)
            {
                var indexes = await _queryStore.GetIndexes(this.DefinitionId);

                DragqnLDIndexDefiniton index;
                var succ = indexes.Indexes.TryGetValue(indexName, out index);
                if (!succ)
                {
                    throw new ArgumentOutOfRangeException("indexName",
                        String.Format("index name {0} is not present in query definition {1}", indexName, DefinitionId));
                }
                luceneQuery = _selectAnalyzer.ConvertSparqlToLuceneWithIndex(sparqlQuery, hierarchy, index);  
            }
            else
            {
                luceneQuery = _selectAnalyzer.ConvertSparqlToLuceneNoIndex(sparqlQuery, hierarchy);
            }

            var response = CreateResponseWithObject(luceneQuery);
            return response;
        }

    }
}