using System;
using System.Collections.Generic;
using System.Web.Mvc;
using eHesabim.Core.Engine;

namespace eHesabim.Web.Portal.Engine {
    public class DependencyResolver : IDependencyResolver {
        public object GetService(Type serviceType) {
            try {
                return EngineContext.Current.Resolve(serviceType);
            }
            catch {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            try {
                var type = typeof(IEnumerable<>).MakeGenericType(serviceType);
                return (IEnumerable<object>)EngineContext.Current.Resolve(type);
            }
            catch {
                return null;
            }
        }
    }
}
