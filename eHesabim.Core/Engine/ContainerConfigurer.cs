using System;
using System.Linq;
using Autofac;

namespace eHesabim.Core.Engine
{
    public class ContainerConfigurer
    {
        public virtual void Configure(IEngine engine, ContainerBuilder builder)
        {
            builder.RegisterType<WebHelper>().As<IWebHelper>().SingleInstance();
            builder.RegisterType<WebAppTypeFinder>().As<ITypeFinder>().SingleInstance();

            var typeFinder = new WebAppTypeFinder(new WebHelper());
            var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            var drInstances = drTypes.Select(drType => (IDependencyRegistrar)Activator.CreateInstance(drType)).ToList();

            // sort
            drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            foreach (var dependencyRegistrar in drInstances)
            {
                dependencyRegistrar.Register(builder, typeFinder);
            }

            builder.RegisterInstance(engine).As<IEngine>();
            builder.RegisterInstance(this).As<ContainerConfigurer>();
        }
    }
}
