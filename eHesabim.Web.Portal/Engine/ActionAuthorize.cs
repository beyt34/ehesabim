using System;
using System.Web.Mvc;
using eHesabim.Core.Engine;
using eHesabim.Core.Logging;
using eHesabim.Framework;
using eHesabim.Services;

namespace eHesabim.Web.Portal.Engine {
    /// <summary>The action authorize.</summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class ActionAuthorize : AuthorizeAttribute {
        /// <summary>Initializes a new instance of the <see cref="ActionAuthorize"/> class.</summary>
        /// <param name="permissionType">The permission type.</param>
        public ActionAuthorize(PermissionTypeEnum permissionType) {
            PermissionType = permissionType;
        }

        /// <summary>Gets or sets the permission type.</summary>
        private PermissionTypeEnum PermissionType { get; set; }

        /// <summary>The on authorization.</summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnAuthorization(AuthorizationContext filterContext) {
            // [Cevdet A. Nuhrat][20130109] Burada olmaları gerekiyor. Aksi halde Attribute 1 kere yaratıldıgından ilk yaratan kişinin workContext'i ile çalışıyor.
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            if (workContext == null || workContext.CurrentUser == null) {
                RedirectLogin(filterContext);
                return;
            }

            if (workContext.CurrentUser.Role == 0) {
                RedirectAccessDenied(filterContext);
                return;
            }

            if (workContext.CurrentUser.Role > PermissionType.GetHashCode()) {
                RedirectAccessDenied(filterContext);
                return;
            }

            base.OnAuthorization(filterContext);
        }

        /// <summary>The redirect login.</summary>
        /// <param name="authorizationContext">The authorization context.</param>
        private void RedirectLogin(AuthorizationContext authorizationContext) {
            var logging = EngineContext.Current.Resolve<ILogging>();
            logging.Trace("RedirectLogin");

            authorizationContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary { { "controller", "Login" }, { "action", "Index" } });
        }

        /// <summary>The redirect access denied.</summary>
        /// <param name="authorizationContext">The authorization context.</param>
        private void RedirectAccessDenied(AuthorizationContext authorizationContext) {
            var logging = EngineContext.Current.Resolve<ILogging>();
            logging.Trace("RedirectAccessDenied");

            authorizationContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary { { "controller", "Base" }, { "action", "AccessDenied" } });
        }
    }
}
