using System.Linq;

using Autofac;
using Autofac.Integration.Mvc;
using eHesabim.Core.Data;
using eHesabim.Core.Engine;
using eHesabim.Core.Logging;
using eHesabim.Core.Token;
using eHesabim.Data;
using eHesabim.Services;

namespace eHesabim.Console {
    public class DependencyRegistrar : IDependencyRegistrar {
        public int Order {
            get { return 0; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder) {
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            // token
            builder.RegisterType<Tokenizer>().As<ITokenizer>().SingleInstance();

            // logging
            builder.RegisterType<Logging>().As<ILogging>().SingleInstance();

            // data
            builder.Register<IDataContext>(c => new DataContext()).InstancePerDependency();
            builder.RegisterGeneric(typeof(ReadOnlyRepository<,>)).As(typeof(IReadOnlyRepository<,>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(GenericRepository<,>)).As(typeof(IRepository<,>)).InstancePerDependency();
            builder.RegisterType<ProcedureManager>().As<IProcedureManager>().InstancePerDependency();

            // services
            builder.RegisterType<CommonService>().As<ICommonService>().InstancePerDependency();
            builder.RegisterType<MessageService>().As<IMessageService>().InstancePerDependency();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerDependency();
       }
    }
}