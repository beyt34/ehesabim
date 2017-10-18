using System;
using System.Linq;

namespace eHesabim.Core.Engine {
    public class ContainerConfigurer {
        public virtual void Configure(IEngine engine, ContainerManager containerManager) {
            // register dependencies provided by other assemblies
            containerManager.AddComponent<IWebHelper, WebHelper>("eHesabim.webHelper");
            containerManager.AddComponent<ITypeFinder, WebAppTypeFinder>("eHesabim.typeFinder");
            var typeFinder = containerManager.Resolve<ITypeFinder>();
            containerManager.UpdateContainer(x => {
                var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
                var drInstances = drTypes.Select(drType => (IDependencyRegistrar)Activator.CreateInstance(drType)).ToList();

                // sort
                drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
                foreach (var dependencyRegistrar in drInstances) {
                    dependencyRegistrar.Register(x, typeFinder);
                }
            });

            containerManager.AddComponentInstance<IEngine>(engine, "eHesabim.engine");
            containerManager.AddComponentInstance<ContainerConfigurer>(this, "eHesabim.containerConfigurer");
        }
    }
}
