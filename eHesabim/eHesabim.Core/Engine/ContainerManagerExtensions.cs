using System.Web;

using Autofac;

namespace eHesabim.Core.Engine {
    public static class ContainerManagerExtensions {
        public static Autofac.Builder.IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> PerLifeStyle<TLimit, TActivatorData, TRegistrationStyle>(this Autofac.Builder.IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder, ComponentLifeStyle lifeStyle) {
            switch (lifeStyle) {
                case ComponentLifeStyle.LifetimeScope:
                    return HttpContext.Current != null ? builder.InstancePerRequest() : builder.InstancePerLifetimeScope();
                case ComponentLifeStyle.Transient:
                    return builder.InstancePerDependency();
                case ComponentLifeStyle.Singleton:
                    return builder.SingleInstance();
                default:
                    return builder.SingleInstance();
            }
        }
    }
}
