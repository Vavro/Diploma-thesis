using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;
using DragqnLD.WebApi.Connection;
using DragqnLD.WebApi.Constants;
using log4net;
using Microsoft.Practices.Unity;
using Raven.Client;
using Raven.Json.Linq;

namespace DragqnLD.WebApi.Controllers
{
    /// <summary>
    /// Base api controller
    /// </summary>
    public abstract class BaseApiController : ApiController
    {
        /// <summary>
        /// The logger
        /// </summary>
        protected static readonly ILog Log = GetCurrentClassLogger();

        private static ILog GetCurrentClassLogger()
        {
            var stackFrame = new StackFrame(1, false);
            return LogManager.GetLogger(stackFrame.GetMethod().DeclaringType);
        }

        /// <summary>
        /// Ravendb Document Store to be used.
        /// </summary>
        /// <value>
        /// The document store.
        /// </value>
        [Dependency]
        public IDocumentStore Store { get; set; }

        //todo: change architecture to Command based, and make commands accept the Session for this request?
        /// <summary>
        /// Gets the ravendb session to be used with this request.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public IAsyncDocumentSession Session { get; private set; }

        /// <summary>
        /// Executes asynchronously a single HTTP operation.
        /// </summary>
        /// <param name="controllerContext">The controller context for a single HTTP operation.</param>
        /// <param name="cancellationToken">The cancellation token assigned for the HTTP operation.</param>
        /// <returns>
        /// The newly started task.
        /// </returns>
        public async override Task<HttpResponseMessage> ExecuteAsync(
            HttpControllerContext controllerContext,
            CancellationToken cancellationToken)
        {
            var values = controllerContext.RequestContext.RouteData.Values;
            if (values.ContainsKey("MS_SubRoutes"))
            {
                var routeDatas = (IHttpRouteData[])controllerContext.Request.GetRouteData().Values["MS_SubRoutes"];
                var selectedData = routeDatas.FirstOrDefault(data => data.Values.ContainsKey("definitionId"));

                if (selectedData != null)
                {
                    selectedData.Values["definitionId"] = Prefixes.Query + selectedData.Values["definitionId"];
                    DefinitionId = selectedData.Values["definitionId"] as string;
                }
                else
                {
                    DefinitionId = null;
                }
            }
            else
            {
                if (values.ContainsKey("definitionId"))
                {
                    values["definitionId"] = Prefixes.Query + values["definitionId"];
                    DefinitionId = values["definitionId"] as string;
                }
                else
                {
                    DefinitionId = null;
                }
            }

            using (Session = Store.OpenAsyncSession())
            {
                var result = await base.ExecuteAsync(controllerContext, cancellationToken);
                await Session.SaveChangesAsync();

                return result;
            }
        }

        /// <summary>
        /// Gets the definition identifier from the route.
        /// </summary>
        /// <value>
        /// The definition identifier.
        /// </value>
        public string DefinitionId { get; private set; }

        /// <summary>
        /// Creates the response.
        /// </summary>
        /// <param name="status">The HTTP status code.</param>
        /// <returns></returns>
        protected HttpResponseMessage CreateResponse(HttpStatusCode status = HttpStatusCode.OK)
        {
            Debug.Assert(Request != null, "Request is null");

            //todo: why do i have to fill an empty object? (maybe its the return type?)
            return Request.CreateResponse(status, new object());
        }


        /// <summary>
        /// Creates the response with object in content.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="status">The HTTP status code.</param>
        /// <returns></returns>
        protected HttpResponseMessage CreateResponseWithObject<T>(T obj, HttpStatusCode status = HttpStatusCode.OK) where T : class
        {
            Debug.Assert(Request != null, "Request is null");

            var response = Request.CreateResponse(status, obj);
            
            return response;
        }

        /// <summary>
        /// Creates the json response.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        protected HttpResponseMessage CreateJsonResponse(RavenJObject obj, HttpStatusCode status = HttpStatusCode.OK)
        {
            Debug.Assert(Request != null, "Request is null");

            var response = Request.CreateResponse(status);
            response.Content = JsonContent(obj);

            return response;
        }

        /// <summary>
        /// Creates the unescaped json response.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="mappings">The mappings.</param>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        protected HttpResponseMessage CreateUnescapedJsonResponse(RavenJObject obj, PropertyMapForUnescape mappings, HttpStatusCode status = HttpStatusCode.OK)
        {
            Debug.Assert(Request != null, "Request is null");

            var response = Request.CreateResponse(status);
            response.Content = UnescapedJsonContent(obj, mappings);

            return response;
        }

        /// <summary>
        /// Converts the data to a Json content.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private JsonContent JsonContent(RavenJToken data = null)
        {
            return new JsonContent(data);
        }

        /// <summary>
        /// Converts the data to a Json content unescaping property names with provided mappings.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="mappings">The mappings.</param>
        /// <returns></returns>
        private UnescapingJsonContent UnescapedJsonContent(RavenJObject content, PropertyMapForUnescape mappings)
        {
            return new UnescapingJsonContent(content, mappings);
        }
    }
}