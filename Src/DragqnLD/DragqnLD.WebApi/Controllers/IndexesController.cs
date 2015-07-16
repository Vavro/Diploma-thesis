﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DragqnLD.Core.Abstraction;
using DragqnLD.WebApi.Models;

namespace DragqnLD.WebApi.Controllers
{
    /// <summary>
    /// Indexes controller for index manipulation tasks
    /// </summary>
    public class IndexesController : BaseApiController
    {
        private readonly IQueryStore _dataStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController"/> class.
        /// </summary>
        /// <param name="dataStore">The data store.</param>
        public IndexesController(IQueryStore dataStore)
        {
            _dataStore = dataStore;
        }

        /// <summary>
        /// Gets the indexes.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/indexes")]
        public async Task<HttpResponseMessage> GetIndexes()
        {
            return CreateResponse(HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Gets the index definition.
        /// </summary>
        /// <param name="indexId">The index identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/index/{*indexId}")]
        public async Task<HttpResponseMessage> GetIndex(string indexId)
        {
            return CreateResponse(HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Stores the index.
        /// </summary>
        /// <param name="indexDefinition">The index definition.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/query/{definitionId}/index")]
        public async Task<HttpResponseMessage> StoreIndex([FromBody] IndexDefinitionDto indexDefinition)
        {
            return CreateResponse(HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Stores or updates the index.
        /// </summary>
        /// <param name="indexId">The index identifier.</param>
        /// <param name="indexDefinition">The index definition.</param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/query/{definitionId}/index/{*indexId}")]
        public async Task<HttpResponseMessage> StoreOrUpdateIndex(string indexId, [FromBody] IndexDefinitionDto indexDefinition)
        {
            return CreateResponse(HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Deletes the index.
        /// </summary>
        /// <param name="indexId">The index identifier.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/query/{definitionId}/index/{*indexId}")]
        public async Task<HttpResponseMessage> DeleteIndex(string indexId)
        {
            return CreateResponse(HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Proposes the index for properties.
        /// </summary>
        /// <param name="propertiesToIndex">Index of the properties to.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/query/{definitionId}/proposeIndex")]
        public async Task<HttpResponseMessage> ProposeIndexForProperties([FromBody] PropertiesToIndexDto propertiesToIndex )
        {
            return CreateResponse(HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Proposes the index for sparql.
        /// </summary>
        /// <param name="sparql">The sparql.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/query/{definitionId}/proposeIndexForSparql")]
        public async Task<HttpResponseMessage> ProposeIndexForSparql([FromBody] SparqlDto sparql)
        {
            return CreateResponse(HttpStatusCode.NotImplemented);
        }
    }
}
