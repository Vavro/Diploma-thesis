using System.Web.Http;
using System.Web.Http.Cors;
using DragqnLD.WebApi.App_Start;
using DragqnLD.WebApi.Filters;

namespace DragqnLD.WebApi
{
    public static class WebApiConfig
    {
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

            AutoMapperConfig.ConfigureMapper();
        }
    }
}
