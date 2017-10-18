using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using eHesabim.Core.Engine;
using eHesabim.Core.Logging;
using eHesabim.Core.Routes;
using eHesabim.Web.Portal.Engine;

namespace eHesabim.Web.Portal {
    public class MvcApplication : HttpApplication {
        protected void Application_Start() {
            EngineContext.Initialize(false);

            var dependencyResolver = new Engine.DependencyResolver();
            System.Web.Mvc.DependencyResolver.SetResolver(dependencyResolver);

            AreaRegistration.RegisterAllAreas();

            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        private static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new BaseHandleErrorAttribute());
        }

        private static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // register custom routes (plugins, etc)
            var routePublisher = EngineContext.Current.Resolve<IRoutePublisher>();
            routePublisher.RegisterRoutes(routes);
        }
    }
}