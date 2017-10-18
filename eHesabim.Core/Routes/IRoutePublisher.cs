using System.Web.Routing;

namespace eHesabim.Core.Routes {
    public interface IRoutePublisher {
        void RegisterRoutes(RouteCollection routeCollection);
    }
}
