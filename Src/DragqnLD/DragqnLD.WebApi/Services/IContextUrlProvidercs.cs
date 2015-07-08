using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;

namespace DragqnLD.WebApi.Services
{

    /// <summary>
    /// Provides urls of context for the core, as it could be run in another container
    /// </summary>
    public interface IContextUrlProvider
    {
        /// <summary>
        /// Gets the URL of the Context of the specified definiton.
        /// </summary>
        /// <param name="definitionId">The definition identifier.</param>
        /// <param name="url"></param>
        /// <returns></returns>
        string GetUrlFor(string definitionId, UrlHelper url);
    }
}
