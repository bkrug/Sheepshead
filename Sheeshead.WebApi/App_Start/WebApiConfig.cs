using Sheepshead.Models;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;
using Sheeshead.WebApi.Json;

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

            config.Services.Insert(typeof(ModelBinderProvider), 0, 
                new SimpleModelBinderProvider(typeof(GameStartModel), new JsonBodyModelBinder<GameStartModel>()));
        }
    }
}
