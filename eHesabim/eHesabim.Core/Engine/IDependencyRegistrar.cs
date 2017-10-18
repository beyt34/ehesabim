using Autofac;

namespace eHesabim.Core.Engine {
    public interface IDependencyRegistrar {
        int Order { get; }

        void Register(ContainerBuilder builder, ITypeFinder typeFinder);
    }
}
