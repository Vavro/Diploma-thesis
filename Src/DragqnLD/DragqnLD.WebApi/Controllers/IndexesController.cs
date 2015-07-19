using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using DragqnLD.Core.Abstraction.Indexes;
using DragqnLD.WebApi.Models;

namespace DragqnLD.WebApi.Controllers
{
    /// <summary>
    /// Indexes controller for index manipulation tasks
    /// </summary>
    public class IndexesController : BaseApiController
    {
        private readonly IQueryStore _queryStore;
        private readonly ISelectAnalyzer _selectAnalyzer;
        private readonly IIndexDefinitionCreater _indexDefinitionCreater;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController" /> class.
        /// </summary>
        /// <param name="queryStore">The data store.</param>
        /// <param name="selectAnalyzer">The select analyzer.</param>
        /// <param name="indexDefinitionCreater">The index definition creater.</param>
        public IndexesController(IQueryStore queryStore, 
            ISelectAnalyzer selectAnalyzer, 
            IIndexDefinitionCreater indexDefinitionCreater)
        {
            _queryStore = queryStore;
            _selectAnalyzer = selectAnalyzer;
            _indexDefinitionCreater = indexDefinitionCreater;
        }

        /// <summary>
        /// Gets the indexes.
        /// </summary>
        /// <returns>List of IndexMetadataDto</returns>
        [HttpGet]
        [Route("api/query/{definitionId}/indexes")]
        public async Task<HttpResponseMessage> GetIndexes()
        {
            var definitionId = DefinitionId;
            var indexes = await _queryStore.GetIndexes(definitionId);

            var indexMetaDto = Mapper.Map<DragqnLDIndexDefinitions, IndexDefinitionsDto>(indexes);

            return CreateResponseWithObject(indexMetaDto);
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
            var id = Mapper.Map<DragqnLDIndexDefiniton>(indexDefinition);
            var indexId = await _queryStore.StoreIndex(this.DefinitionId, id);
            return CreateResponse();
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
        public async Task<HttpResponseMessage> ProposeIndexForProperties([FromBody] PropertiesToIndexDto propertiesToIndex)
        {
            var definition = await _queryStore.Get(this.DefinitionId);
            var accessibleProperties = await _queryStore.GetHierarchy(this.DefinitionId);
            var requirements = Mapper.Map<DragqnLDIndexRequirements>(propertiesToIndex);
            var proposedIndex = _indexDefinitionCreater.CreateIndexDefinitionFor(definition, accessibleProperties, requirements);

            var proposedIndexDto = Mapper.Map<IndexDefinitionDto>(proposedIndex);

            return CreateResponseWithObject(proposedIndexDto);
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

        /// <summary>
        /// Gets the indexes.
        /// </summary>
        /// <returns>List of IndexMetadataDto</returns>
        [HttpGet]
        [Route("api/query/{definitionId}/indexableProperties")]
        public async Task<HttpResponseMessage> GetIndexableProperties()
        {
            var definitionId = DefinitionId;
            var hierarchy = await _queryStore.GetHierarchy(definitionId);

            var indexableProperties = hierarchy.RootProperty.GetPropertyPaths();

            var indexablePropertiesDto = new IndexablePropertiesDto() { Properties = indexableProperties };

            return CreateResponseWithObject(indexablePropertiesDto);
        }

    }
}
