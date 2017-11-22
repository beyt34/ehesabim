using System.Web.Mvc;
using System.Web.Routing;

using eHesabim.Core.Engine;
using eHesabim.Core.Logging;
using eHesabim.Core.Routes;
using eHesabim.Web.Task.Engine;

namespace eHesabim.Web.Task {
    public class MvcApplication : System.Web.HttpApplication {
        protected void Application_Start() {
            EngineContext.Initialize(false);
            DependencyResolver.SetResolver(new Framework.DependencyResolver());

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            TaskManager.Instance.Initialize();
            TaskManager.Instance.Start();
        }

        protected void Application_End() {
            TaskManager.Instance.Stop();
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