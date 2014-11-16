using System;
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
using DragqnLD.WebApi.Configuration;
using log4net;
using Raven.Client;
using Raven.Client.Document;

namespace DragqnLD.WebApi.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        protected static readonly ILog Log = GetCurrentClassLogger();

        private static ILog GetCurrentClassLogger()
        {
            var stackFrame = new StackFrame(1, false);
            return LogManager.GetLogger(stackFrame.GetMethod().DeclaringType);
        }

        public IDocumentStore Store
        {
            get { return LazyDocStore.Value; }
        }

        private static readonly Lazy<IDocumentStore> LazyDocStore = new Lazy<IDocumentStore>(() =>
        {
            var docStore = new DocumentStore
            {
                Url = DragqnLdConfig.Instance.DatabaseUrl,
                DefaultDatabase = DragqnLdConfig.Instance.DatabaseName
            };

            docStore.Initialize();
            return docStore;
        });

        public IAsyncDocumentSession Session { get; set; }

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
                    selectedData.Values["definitionId"] = "Query/" + selectedData.Values["definitionId"];
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
                    values["definitionId"] = "Query/" + values["definitionId"];
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

        public string DefinitionId { get; private set; }

        protected HttpResponseMessage CreateResponse(HttpStatusCode status = HttpStatusCode.OK)
        {
            Debug.Assert(Request != null, "Request is null");

            //todo: why do i have to fill an empty object? (maybe its the return type?)
            return Request.CreateResponse(status, new object());
        }


        protected HttpResponseMessage CreateResponseWithObject<T>(T obj, HttpStatusCode status = HttpStatusCode.OK)
        {
            Debug.Assert(Request != null, "Request is null");

            var response = Request.CreateResponse(status, obj);

            return response;
        }
    }
}