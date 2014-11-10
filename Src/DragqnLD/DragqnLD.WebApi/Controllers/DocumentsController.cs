using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Implementations;

namespace DragqnLD.WebApi.Controllers
{
    public class DocumentsController : BaseApiController
    {
        private readonly IDataStore _dataStore;

        public DocumentsController()
        {
            _dataStore = new RavenDataStore(Store, new DocumentPropertyEscaper());
        }

        [HttpGet]
        [Route("api/query/{definitionId}/documents")]
        public async Task<HttpResponseMessage> Get()
        {
            return Request.CreateResponse(HttpStatusCode.NotImplemented);
        }

        [HttpGet]
        [Route("api/query/{definitionId}/document")]
        public async Task<HttpResponseMessage> Get(string documentId)
        {
            var document = await _dataStore.GetDocument(DefinitionId, new Uri(documentId));

            //todo: get rid of Content property write raw json to response
            return Request.CreateResponse(HttpStatusCode.OK, document);
        }
    }
}
