using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Antlr.Runtime.Tree;
using AutoMapper;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.WebApi.Models;
using DragqnLD.WebApi.Services;
using Raven.Json.Linq;

namespace DragqnLD.WebApi.Controllers
{
    /// <summary>
    /// Controller for reading Documents stored for a Query Definition
    /// </summary>
    public class DocumentsController : BaseApiController
    {
        private readonly IDataStore _dataStore;
        private IContextUrlProvider _contextUrlProvider ;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentsController"/> class.
        /// </summary>
        /// <param name="dataStore">The data store.</param>
        /// <param name="contextUrlProvider">the context url provider for injecting @context urls into the documents</param>
        public DocumentsController(IDataStore dataStore, IContextUrlProvider contextUrlProvider)
        {
            _dataStore = dataStore;
            _contextUrlProvider = contextUrlProvider;
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
            var documentsWithMaps = await _dataStore.GetDocumentWithMappings(DefinitionId, new Uri(documentId), true);
            var document = documentsWithMaps.Item1;
            var mappings = documentsWithMaps.Item2;

            //done: add context link to response -- would be better if it would be first but its not against the syntax
            var contextUrl = _contextUrlProvider.GetUrlFor(DefinitionId, Url);
            document.Content.Add("@context", new RavenJValue(contextUrl));

            //done: get rid of Content property write raw json to response
            //todo: leaks the mapping and escaping from the core library - but is the only performant way?

            //done: Unescape document!
            //done: create cache for mappings
            var response = CreateUnescapedJsonResponse(document.Content, mappings);

            return response;
        }
    }
}
