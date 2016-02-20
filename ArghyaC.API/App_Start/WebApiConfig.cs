using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Routing;

namespace ArghyaC.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            var routes = config.Routes;
            routes.MapHttpRoute("RestApiRoute", "Api/{controller}/{id}", new { id = RouteParameter.Optional }, new { id = @"\d+" });
            routes.MapHttpRoute("ApiWithActionRoute", "Api/{controller}/{action}/{id}", new { id = RouteParameter.Optional });
            routes.MapHttpRoute("DefaultApiGetRoute", "Api/{controller}", new { action = "Get" }, new { httpMethod = new HttpMethodConstraint(new string[] { "GET" }) });
            routes.MapHttpRoute("DefaultApiPostRoute", "Api/{controller}", new { action = "Post" }, new { httpMethod = new HttpMethodConstraint(new string[] { "POST" }) });
            routes.MapHttpRoute("DefaultApiPutRoute", "Api/{controller}", new { action = "Put" }, new { httpMethod = new HttpMethodConstraint(new string[] { "PUT" }) });
            routes.MapHttpRoute("DefaultApiDeleteRoute", "Api/{controller}", new { action = "Delete" }, new { httpMethod = new HttpMethodConstraint(new string[] { "DELETE" }) });

            //Making json the default formatter. It stops rendering html. Comment in case html needs to be served
            //config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            // 2 Removes xml formatter. Comment if xml needs to be served
            //GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            //config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.Formatters.JsonFormatter.MediaTypeMappings.Add(new QueryStringMapping("format", "json", "application/json"));
            config.Formatters.XmlFormatter.MediaTypeMappings.Add(new QueryStringMapping("format", "xml", "application/xml"));
            //config.Formatters.JsonFormatter.MediaTypeMappings.Add(new RequestHeaderMapping("Accept", "text/html", StringComparison.InvariantCultureIgnoreCase, true, "application/json"));


            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();
        }
    }
}
