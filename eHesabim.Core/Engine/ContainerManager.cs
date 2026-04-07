using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace eHesabim.Core.Engine
{
    public class ContainerManager
    {
        private readonly IContainer container;

        public ContainerManager(IContainer container)
        {
            this.container = container;
        }

        public IContainer Container
        {
            get { return container; }
        }

        public T Resolve<T>(string key = "") where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                return Scope().Resolve<T>();
            }

            return Scope().ResolveKeyed<T>(key);
        }

        public object Resolve(Type type)
        {
            return Scope().Resolve(type);
        }

        public T[] ResolveAll<T>(string key = "")
        {
            if (string.IsNullOrEmpty(key))
            {
                return Scope().Resolve<IEnumerable<T>>().ToArray();
            }

            return Scope().ResolveKeyed<IEnumerable<T>>(key).ToArray();
        }

        public T ResolveUnregistered<T>() where T : class
        {
            return ResolveUnregistered(typeof(T)) as T;
        }

        public object ResolveUnregistered(Type type)
        {
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters();
                    var parameterInstances = new List<object>();
                    foreach (var parameter in parameters)
                    {
                        var service = Resolve(parameter.ParameterType);
                        if (service == null)
                        {
                            throw new Exception("Unkown dependency");
                        }

                        parameterInstances.Add(service);
                    }

                    return Activator.CreateInstance(type, parameterInstances.ToArray());
                }
                catch (Exception)
                {
                }
            }

            throw new Exception("No contructor was found that had all the dependencies satisfied.");
        }

        public ILifetimeScope Scope()
        {
            try
            {
                return AutofacRequestLifetimeHttpModule.GetLifetimeScope(Container, null);
            }
            catch
            {
                return Container;
            }
        }
    }
}
