using System.Linq;
using System.Web;
using System.Web.Mvc;

using Autofac;
using Autofac.Integration.Mvc;

using eHesabim.Core.Data;
using eHesabim.Core.Engine;
using eHesabim.Core.Logging;
using eHesabim.Core.Routes;
using eHesabim.Core.Token;
using eHesabim.Data;
using eHesabim.Framework.Localization;

namespace eHesabim.Framework {
    public class DependencyRegistrar : IDependencyRegistrar {
        public int Order {
            get { return 0; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder) {
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            ////builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerDependency();
            
            // token
            builder.RegisterType<Tokenizer>().As<ITokenizer>().SingleInstance();

            // logging
            builder.RegisterType<Logging>().As<ILogging>().SingleInstance();

            // data
            builder.Register<IDataContext>(c => new DataContext()).InstancePerDependency();
            builder.RegisterGeneric(typeof(ReadOnlyRepository<,>)).As(typeof(IReadOnlyRepository<,>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(GenericRepository<,>)).As(typeof(IRepository<,>)).InstancePerDependency();
            builder.RegisterType<ProcedureManager>().As<IProcedureManager>().InstancePerDependency();

            // HttpContext
            builder.Register(c => new HttpContextWrapper(HttpContext.Current) as HttpContextBase);
            builder.Register(c => c.Resolve<HttpContextBase>().Request).As<HttpRequestBase>().InstancePerDependency();
            builder.Register(c => c.Resolve<HttpContextBase>().Response).As<HttpResponseBase>().InstancePerDependency();
            builder.Register(c => c.Resolve<HttpContextBase>().Server).As<HttpServerUtilityBase>().InstancePerDependency();
            builder.Register(c => c.Resolve<HttpContextBase>().Session).As<HttpSessionStateBase>().InstancePerDependency();

            // validation override
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(ResourceRequiredAttribute), typeof(RequiredAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(ResourceStringLengthAttribute), typeof(StringLengthAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(ResourceRegularExpressionAttribute), typeof(RegularExpressionAttributeAdapter));

            // work context
            builder.RegisterType<WebWorkContext>().As<IWorkContext>().InstancePerDependency();

            // The End
            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();
        }
    }
}