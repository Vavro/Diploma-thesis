using System.Web.Http;
using System.Web.Http.Cors;
using DragqnLD.WebApi.App_Start;
using DragqnLD.WebApi.Filters;
using DragqnLD.WebApi.MediaFormatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DragqnLD.WebApi
{
    /// <summary>
    /// Web Api confiration
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers the WebAPI into the specified configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            
            config.Routes.MapHttpRoute(
                name: "QueriesApiController",
                routeTemplate: "api/query/{definitionId}",
                defaults: new { controller = "Queries", definitionId = RouteParameter.Optional }
            );

            //todo: delete when embedded in querydef storing
            config.Routes.MapHttpRoute(
                name: "Process query",
                routeTemplate: "api/process/{definitionId}",
                defaults: new {controller = "Tasks", action = "Process"}
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            config.EnableCors(new EnableCorsAttribute("*","*","*"));

            config.Filters.Add(new ValidateModelFilter());

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Add(new IsoDateTimeConverter());
            config.Formatters[0] = new JsonNetFormatter(serializerSettings);

            AutoMapperConfig.ConfigureMapper();
        }
    }
}
