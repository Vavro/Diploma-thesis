﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
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
                Url = "http://localhost:8080",
                DefaultDatabase = "WebApiSample"
            };

            docStore.Initialize();
            return docStore;
        });

        public IAsyncDocumentSession Session { get; set; }

        public async override Task<HttpResponseMessage> ExecuteAsync(
            HttpControllerContext controllerContext,
            CancellationToken cancellationToken)
        {
            using (Session = Store.OpenAsyncSession())
            {
                var result = await base.ExecuteAsync(controllerContext, cancellationToken);
                await Session.SaveChangesAsync();

                return result;
            }
        }

    }
}