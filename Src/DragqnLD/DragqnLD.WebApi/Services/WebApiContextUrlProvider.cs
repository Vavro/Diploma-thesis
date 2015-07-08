using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Http.Routing;
using DragqnLD.Core.Abstraction;
using DragqnLD.WebApi.Constants;

namespace DragqnLD.WebApi.Services
{
    /// <summary>
    /// Provides urls of context for the core, soecificaly to the WEB API in which this is run
    /// </summary>
    public class WebApiContextUrlProvider : IContextUrlProvider
    {
        /// <summary>
        /// Gets the URL of the Context of the specified definiton.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetUrlFor(string definitionId, UrlHelper url)
        {
            var intId = definitionId.Substring(Prefixes.Query.Length - 1);

            var link = url.Link(RouteNames.Contexts, intId);

            return link;
        }
    }
}