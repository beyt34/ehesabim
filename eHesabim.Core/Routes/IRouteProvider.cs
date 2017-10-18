using System.Web.Routing;

namespace eHesabim.Core.Routes {
    public interface IRouteProvider {
        int Priority { get; }

        void RegisterRoutes(RouteCollection routes);
    }
}
