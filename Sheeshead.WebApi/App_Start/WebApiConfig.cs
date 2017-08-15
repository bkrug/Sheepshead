using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Routing;

namespace Sheeshead.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var cors = new EnableCorsAttribute("http://localhost:1337", "*", "*");
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            ////The idea was that this code would make the GameController routes operate
            ////more like an MVC controller, but they didn't seem to work.
            //config.Routes.MapHttpRoute("DefaultApiWithId", "Api/{controller}/{id}", 
            //    new { id = RouteParameter.Optional }, 
            //    new { id = @"\d+" });
            //config.Routes.MapHttpRoute("DefaultApiWithAction", "Api/{controller}/{action}");
            //config.Routes.MapHttpRoute("DefaultApiGet", "Api/{controller}", 
            //    new { action = "Get" }, 
            //    new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            //config.Routes.MapHttpRoute("DefaultApiPost", "Api/{controller}/{action}", 
            //    new { action = "Post" }, 
            //    new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
        }
    }
}