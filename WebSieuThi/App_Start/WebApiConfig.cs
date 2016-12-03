using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Routing;

namespace WebSieuThi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute("DefaultApiWithId", "api/{controller}/{id}",
                new { id = RouteParameter.Optional },
                new { id = @"\d+" }
            );

            config.Routes.MapHttpRoute("DefaultApiWithActionAndId", "api/{controller}/{action}/{id}",
                new { id = RouteParameter.Optional },
                new { id = @"\d+" }
            );

            config.Routes.MapHttpRoute("DefaultApiWithAction", "api/{controller}/{action}");

            config.Routes.MapHttpRoute("DefaultApiGet", "api/{controller}",
                new { action = "Get" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

            config.Routes.MapHttpRoute("DefaultApiPost", "api/{controller}",
                new { action = "Post" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}
