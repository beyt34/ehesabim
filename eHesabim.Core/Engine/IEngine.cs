using System;

namespace eHesabim.Core.Engine {
    public interface IEngine {
        ContainerManager ContainerManager { get; }

        void Initialize();

        T Resolve<T>() where T : class;

        object Resolve(Type type);

        T[] ResolveAll<T>();
    }
}
