using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Antlr.Runtime.Tree;
using AutoMapper;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.WebApi.Models;
using Raven.Json.Linq;

namespace DragqnLD.WebApi.Controllers
{
    /// <summary>
    /// Controller for reading Documents stored for a Query Definition
    /// </summary>
    public class DocumentsController : BaseApiController
    {
        private readonly IDataStore _dataStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentsController"/> class.
        /// </summary>
        /// <param name="dataStore">The data store.</param>
        public DocumentsController(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        /// <summary>
        /// Gets this document metadata of this query definition id.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/documents")]
        public async Task<HttpResponseMessage> Get(int start = 0, int pageSize = 20)
        {
            var pagedDocumentMetadata = await _dataStore.GetDocuments(DefinitionId, start, pageSize);
            var pagedDocumentMetadataDto = Mapper.Map<PagedDocumentMetadataDto>(pagedDocumentMetadata);
           
            return CreateResponseWithObject(pagedDocumentMetadataDto);
        }

        /// <summary>
        /// Gets the document for the specified query definition id.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/document")]
        public async Task<HttpResponseMessage> Get(string documentId)
        {
            var documentsWithMaps = await _dataStore.GetDocumentWithMappings(DefinitionId, new Uri(documentId));
            var document = documentsWithMaps.Item1;
            var mappings = documentsWithMaps.Item2;

            //done: get rid of Content property write raw json to response
            //todo: leaks the mapping and escaping from the core library

            //done: Unescape document!
            //done: create cache for mappings
            return CreateUnescapedJsonResponse(document.Content, mappings);
        }
    }
}
