using System;
using System.Web.Mvc;

using eHesabim.Core;
using eHesabim.Core.Engine;

namespace eHesabim.Framework.Security {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class HttpsAttribute : FilterAttribute, IAuthorizationFilter {
        public HttpsAttribute(SslRequirement sslRequirement) {
            SslRequirement = sslRequirement;
        }

        public SslRequirement SslRequirement { get; set; }

        public virtual void OnAuthorization(AuthorizationContext filterContext) {
            if (filterContext == null) {
                throw new ArgumentNullException("filterContext");
            }

            // only redirect for GET requests, otherwise the browser might not propagate the verb and request body correctly.
            if (!string.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase)) {
                return;
            }

            var currentConnectionSecured = filterContext.HttpContext.Request.IsSecureConnection;
            ////currentConnectionSecured = webHelper.IsCurrentConnectionSecured();
            switch (SslRequirement) {
                case SslRequirement.Optional:
                    return;
                case SslRequirement.Yes: {
                        if (!currentConnectionSecured) {
                            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                            if (webHelper.SslEnabled()) {
                                // redirect to HTTPS version of page
                                // string url = "https://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
                                var url = webHelper.GetThisPageUrl(true, true);
                                filterContext.Result = new RedirectResult(url);
                            }
                        }
                    }

                    break;
                case SslRequirement.No: {
                        if (currentConnectionSecured) {
                            var webHelper = EngineContext.Current.Resolve<IWebHelper>();

                            // redirect to HTTP version of page
                            // string url = "http://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
                            var url = webHelper.GetThisPageUrl(true, false);
                            filterContext.Result = new RedirectResult(url);
                        }
                    }

                    break;
                default:
                    throw new Exception("Not supported SslProtected parameter");
            }
        }
    }
}