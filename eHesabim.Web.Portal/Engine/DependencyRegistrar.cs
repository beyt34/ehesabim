using System.Linq;
using Autofac;
using Autofac.Integration.Mvc;
using eHesabim.Core.Engine;
using eHesabim.Core.Routes;
using eHesabim.Core.Storage;
using eHesabim.Framework.Facebook;
using eHesabim.Services;

namespace eHesabim.Web.Portal.Engine {
    public class DependencyRegistrar : IDependencyRegistrar {
        public int Order {
            get { return 0; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder) {
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            // storage
            builder.RegisterType<StorageService>().As<IStorageService>().InstancePerDependency();

            // services
            builder.RegisterType<BankAccountService>().As<IBankAccountService>().InstancePerDependency();
            builder.RegisterType<BankCreditService>().As<IBankCreditService>().InstancePerDependency();
            builder.RegisterType<BankCreditCardService>().As<IBankCreditCardService>().InstancePerDependency();
            builder.RegisterType<CommonService>().As<ICommonService>().InstancePerDependency();
            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerDependency();
            builder.RegisterType<ExpenseService>().As<IExpenseService>().InstancePerDependency();
            builder.RegisterType<MessageService>().As<IMessageService>().InstancePerDependency();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerDependency();

            // facebook
            builder.RegisterType<FacebookService>().As<IFacebookService>().InstancePerDependency();

            // The End
            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();
        }
    }
}
