using System.Linq;

using Autofac;
using Autofac.Integration.Mvc;

using eHesabim.Core.Engine;
using eHesabim.Core.Routes;
using eHesabim.Services;

namespace eHesabim.Web.Task.Engine {
    public class DependencyRegistrar : IDependencyRegistrar {
        public int Order {
            get { return 1; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder) {
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            // services
            builder.RegisterType<MessageService>().As<IMessageService>().InstancePerDependency();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerDependency();

            // the end
            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();
       }
    }
}