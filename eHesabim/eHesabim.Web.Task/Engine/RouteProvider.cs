using System.Web.Mvc;
using System.Web.Routing;

using eHesabim.Core.Routes;

namespace eHesabim.Web.Task.Engine {
    public class RouteProvider : IRouteProvider {
        public int Priority {
            get {
                return 0;
            }
        }

        public void RegisterRoutes(RouteCollection routes) {
            routes.MapRoute("Home", string.Empty, new { controller = "Home", action = "Index" });
            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}