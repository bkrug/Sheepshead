using Sheepshead;
using System.Web.Http;

namespace Sheeshead.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            LoadMoveStatRepository.Load();
        }
    }
}
