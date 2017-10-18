using System;
using System.Web;

using Autofac;
using Autofac.Integration.Mvc;

namespace eHesabim.Core.Engine {
    public class AutofacRequestLifetimeHttpModule : IHttpModule {
        public static readonly object HttpRequestTag = "AutofacWebRequest";

        public static ILifetimeScope LifetimeScope {
            get { return (ILifetimeScope)HttpContext.Current.Items[typeof(ILifetimeScope)]; }
            set { HttpContext.Current.Items[typeof(ILifetimeScope)] = value; }
        }

        public static ILifetimeScope GetLifetimeScope(ILifetimeScope container, Action<ContainerBuilder> configurationAction) {
            ////little hack here to get dependencies when HttpContext is not available
            if (HttpContext.Current != null) {
                return LifetimeScope ?? (LifetimeScope = InitializeLifetimeScope(configurationAction, container));
            }

            ////throw new InvalidOperationException("HttpContextNotAvailable");
            return InitializeLifetimeScope(configurationAction, container);
        }

        public static void ContextEndRequest(object sender, EventArgs e) {
            ILifetimeScope lifetimeScope = LifetimeScope;
            if (lifetimeScope != null) {
                lifetimeScope.Dispose();
            }
        }

        public static ILifetimeScope InitializeLifetimeScope(Action<ContainerBuilder> configurationAction, ILifetimeScope container) {
            return (configurationAction == null)
                ? container.BeginLifetimeScope(HttpRequestTag)
                : container.BeginLifetimeScope(HttpRequestTag, configurationAction);
        }

        public void Init(HttpApplication context) {
            context.EndRequest += ContextEndRequest;
        }

        public void Dispose() {
        }
    }
}