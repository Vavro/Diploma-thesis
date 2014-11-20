using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DragqnLD.Core.Abstraction;

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
        /// <returns></returns>
        [HttpGet]
        [Route("api/query/{definitionId}/documents")]
        public async Task<HttpResponseMessage> Get()
        {
            var documents = await _dataStore.GetDocuments(DefinitionId);

            return CreateResponseWithObject(documents);

            //todo: implement
            return CreateResponse(HttpStatusCode.NotImplemented);
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
            var document = await _dataStore.GetDocument(DefinitionId, new Uri(documentId));

            //todo: get rid of Content property write raw json to response
            return CreateResponseWithObject(document);
        }
    }
}
