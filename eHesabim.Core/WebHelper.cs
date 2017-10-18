using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;

using eHesabim.Core.Engine;
using eHesabim.Core.Logging;

namespace eHesabim.Core {
    public class WebHelper : IWebHelper {
        public virtual string GetUrlReferrer() {
            var referrerUrl = string.Empty;

            if (HttpContext.Current != null &&
                HttpContext.Current.Request.UrlReferrer != null) {
                referrerUrl = HttpContext.Current.Request.UrlReferrer.ToString();
            }

            return referrerUrl;
        }

        public virtual string GetCurrentIpAddress() {
            if (HttpContext.Current != null && HttpContext.Current.Request.UserHostAddress != null) {
                return HttpContext.Current.Request.UserHostAddress;
            }

            return string.Empty;
        }

        public virtual string GetThisPageUrl(bool includeQueryString) {
            var useSsl = IsCurrentConnectionSecured();
            return GetThisPageUrl(includeQueryString, useSsl);
        }

        public virtual string GetThisPageUrl(bool includeQueryString, bool useSsl) {
            var url = string.Empty;
            if (HttpContext.Current == null) {
                return url;
            }

            if (includeQueryString) {
                var storeHost = GetStoreHost(useSsl);
                if (storeHost.EndsWith("/")) {
                    storeHost = storeHost.Substring(0, storeHost.Length - 1);
                }

                url = storeHost + HttpContext.Current.Request.RawUrl;
            }
            else {
                url = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
            }

            url = url.ToLowerInvariant();
            return url;
        }

        public virtual bool IsCurrentConnectionSecured() {
            var useSsl = false;
            if (HttpContext.Current != null) {
                useSsl = HttpContext.Current.Request.IsSecureConnection;

                // when your hosting uses a load balancer on their server then the Request.IsSecureConnection is never got set to true, use the statement below
                // just uncomment it
                // useSSL = HttpContext.Current.Request.ServerVariables["HTTP_CLUSTER_HTTPS"] == "on" ? true : false;
            }

            return useSsl;
        }

        public virtual bool SslEnabled() {
            var useSsl = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["UseSSL"]) && Convert.ToBoolean(ConfigurationManager.AppSettings["UseSSL"]);
            return useSsl;
        }

        public virtual string ServerVariables(string name) {
            var tmpS = string.Empty;
            try {
                if (HttpContext.Current.Request.ServerVariables[name] != null) {
                    tmpS = HttpContext.Current.Request.ServerVariables[name];
                }
            }
            catch {
                tmpS = string.Empty;
            }

            return tmpS;
        }

        public virtual string GetStoreHost(bool useSsl) {
            var result = "http://" + ServerVariables("HTTP_HOST");
            if (!result.EndsWith("/")) {
                result += "/";
            }

            if (useSsl) {
                // shared SSL certificate URL
                var sharedSslUrl = string.Empty;
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SharedSSLUrl"])) {
                    sharedSslUrl = ConfigurationManager.AppSettings["SharedSSLUrl"].Trim();
                }

                result = !string.IsNullOrEmpty(sharedSslUrl) ? sharedSslUrl : result.Replace("http:/", "https:/");
            }
            else {
                if (SslEnabled()) {
                    // get shared SSL certificate URL
                    var sharedSslUrl = string.Empty;
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SharedSSLUrl"])) {
                        sharedSslUrl = ConfigurationManager.AppSettings["SharedSSLUrl"].Trim();
                    }

                    if (!string.IsNullOrEmpty(sharedSslUrl)) {
                        var nonSharedSslUrl = string.Empty;
                        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["NonSharedSSLUrl"])) {
                            nonSharedSslUrl = ConfigurationManager.AppSettings["NonSharedSSLUrl"].Trim();
                        }

                        if (string.IsNullOrEmpty(nonSharedSslUrl)) {
                            throw new Exception("NonSharedSSLUrl app config setting is not empty");
                        }

                        result = nonSharedSslUrl;
                    }
                }
            }

            if (!result.EndsWith("/")) {
                result += "/";
            }

            return result.ToLowerInvariant();
        }

        public virtual string GetStoreLocation() {
            var useSsl = IsCurrentConnectionSecured();
            return GetStoreLocation(useSsl);
        }

        public virtual string GetStoreLocation(bool useSsl) {
            var result = GetStoreHost(useSsl);
            if (result.EndsWith("/")) {
                result = result.Substring(0, result.Length - 1);
            }

            result = result + HttpContext.Current.Request.ApplicationPath;
            if (!result.EndsWith("/")) {
                result += "/";
            }

            return result.ToLowerInvariant();
        }

        public virtual bool IsStaticResource(HttpRequest request) {
            if (request == null) {
                throw new ArgumentNullException("request");
            }

            var path = request.Path;
            var extension = VirtualPathUtility.GetExtension(path);

            if (extension == null) {
                return false;
            }

            switch (extension.ToLower()) {
                case ".axd":
                case ".ashx":
                case ".bmp":
                case ".css":
                case ".gif":
                case ".ico":
                case ".jpeg":
                case ".jpg":
                case ".js":
                case ".png":
                case ".rar":
                case ".zip":
                    return true;
            }

            return false;
        }

        public virtual string MapPath(string path) {
            if (HttpContext.Current != null) {
                return HostingEnvironment.MapPath(path);
            }

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var binIndex = baseDirectory.IndexOf("\\bin\\", StringComparison.Ordinal);
            if (binIndex >= 0) {
                baseDirectory = baseDirectory.Substring(0, binIndex);
            }
            else if (baseDirectory.EndsWith("\\bin")) {
                baseDirectory = baseDirectory.Substring(0, baseDirectory.Length - 4);
            }

            path = path.Replace("~/", string.Empty).TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }

        public virtual string ModifyQueryString(string url, string queryStringModification, string targetLocationModification) {
            if (url == null) {
                url = string.Empty;
            }

            url = url.ToLowerInvariant();

            if (queryStringModification == null) {
                queryStringModification = string.Empty;
            }

            queryStringModification = queryStringModification.ToLowerInvariant();

            if (targetLocationModification == null) {
                targetLocationModification = string.Empty;
            }

            targetLocationModification = targetLocationModification.ToLowerInvariant();

            var str = string.Empty;
            var str2 = string.Empty;
            if (url.Contains("#")) {
                str2 = url.Substring(url.IndexOf("#", StringComparison.Ordinal) + 1);
                url = url.Substring(0, url.IndexOf("#", StringComparison.Ordinal));
            }

            if (url.Contains("?")) {
                str = url.Substring(url.IndexOf("?", StringComparison.Ordinal) + 1);
                url = url.Substring(0, url.IndexOf("?", StringComparison.Ordinal));
            }

            if (!string.IsNullOrEmpty(queryStringModification)) {
                if (!string.IsNullOrEmpty(str)) {
                    var dictionary = new Dictionary<string, string>();
                    foreach (var str3 in str.Split(new[] { '&' })) {
                        if (!string.IsNullOrEmpty(str3)) {
                            var strArray = str3.Split(new[] { '=' });
                            if (strArray.Length == 2) {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else {
                                dictionary[str3] = null;
                            }
                        }
                    }

                    foreach (var str4 in queryStringModification.Split(new[] { '&' })) {
                        if (!string.IsNullOrEmpty(str4)) {
                            var strArray2 = str4.Split(new[] { '=' });
                            if (strArray2.Length == 2) {
                                dictionary[strArray2[0]] = strArray2[1];
                            }
                            else {
                                dictionary[str4] = null;
                            }
                        }
                    }

                    var builder = new StringBuilder();
                    foreach (var str5 in dictionary.Keys) {
                        if (builder.Length > 0) {
                            builder.Append("&");
                        }

                        builder.Append(str5);
                        if (dictionary[str5] != null) {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }

                    str = builder.ToString();
                }
                else {
                    str = queryStringModification;
                }
            }

            if (!string.IsNullOrEmpty(targetLocationModification)) {
                str2 = targetLocationModification;
            }

            return (url + (string.IsNullOrEmpty(str) ? string.Empty : ("?" + str)) + (string.IsNullOrEmpty(str2) ? string.Empty : ("#" + str2))).ToLowerInvariant();
        }

        public virtual string RemoveQueryString(string url, string queryString) {
            if (url == null) {
                url = string.Empty;
            }

            url = url.ToLowerInvariant();

            if (queryString == null) {
                queryString = string.Empty;
            }

            queryString = queryString.ToLowerInvariant();

            var str = string.Empty;
            if (url.Contains("?")) {
                str = url.Substring(url.IndexOf("?", StringComparison.Ordinal) + 1);
                url = url.Substring(0, url.IndexOf("?", StringComparison.Ordinal));
            }

            if (!string.IsNullOrEmpty(queryString)) {
                if (!string.IsNullOrEmpty(str)) {
                    var dictionary = new Dictionary<string, string>();
                    foreach (var str3 in str.Split(new[] { '&' })) {
                        if (!string.IsNullOrEmpty(str3)) {
                            var strArray = str3.Split(new[] { '=' });
                            if (strArray.Length == 2) {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else {
                                dictionary[str3] = null;
                            }
                        }
                    }

                    dictionary.Remove(queryString);

                    var builder = new StringBuilder();
                    foreach (var str5 in dictionary.Keys) {
                        if (builder.Length > 0) {
                            builder.Append("&");
                        }

                        builder.Append(str5);
                        if (dictionary[str5] != null) {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }

                    str = builder.ToString();
                }
            }

            return url + (string.IsNullOrEmpty(str) ? string.Empty : ("?" + str));
        }

        public virtual T QueryString<T>(string name) {
            string queryParam = null;
            if (HttpContext.Current != null && HttpContext.Current.Request.QueryString[name] != null) {
                queryParam = HttpContext.Current.Request.QueryString[name];
            }

            if (!string.IsNullOrEmpty(queryParam)) {
                return CommonHelper.To<T>(queryParam);
            }

            return default(T);
        }

        public virtual void RestartAppDomain(string redirectUrl = "") {
            if (CommonHelper.GetTrustLevel() > AspNetHostingPermissionLevel.Medium) {
                // full trust
                HttpRuntime.UnloadAppDomain();

                TryWriteGlobalAsax();
            }
            else {
                // medium trust
                var success = TryWriteWebConfig();
                if (!success) {
                    throw new Exception("nopCommerce needs to be restarted due to a configuration change, but was unable to do so.\r\n" +
                        "To prevent this issue in the future, a change to the web server configuration is required:\r\n" +
                        "- run the application in a full trust environment, or\r\n" +
                        "- give the application write access to the 'web.config' file.");
                }

                success = TryWriteGlobalAsax();
                if (!success) {
                    throw new Exception("nopCommerce needs to be restarted due to a configuration change, but was unable to do so.\r\n" +
                        "To prevent this issue in the future, a change to the web server configuration is required:\r\n" +
                        "- run the application in a full trust environment, or\r\n" +
                        "- give the application write access to the 'Global.asax' file.");
                }
            }

            // If setting up extensions/modules requires an AppDomain restart, it's very unlikely the
            // current request can be processed correctly.  So, we redirect to the same URL, so that the
            // new request will come to the newly started AppDomain.
            var httpContext = HttpContext.Current;
            if (httpContext != null) {
                if (string.IsNullOrEmpty(redirectUrl)) {
                    redirectUrl = GetThisPageUrl(true);
                }

                httpContext.Response.Redirect(redirectUrl, true /*endResponse*/);
            }
        }

        public virtual bool IsSearchEngine(HttpContextBase context) {
            var logging = EngineContext.Current.Resolve<ILogging>();
            if (context == null) {
                return false;
            }

            var result = false;
            try {
                result = context.Request.Browser.Crawler;

                // Ignore monitoring requests
                if (!result) {
                    result = context.Request.RequestType.ToLower() == "head";
                }

                // Check Generic bots
                if (!result) {
                    var userAgent = context.Request.UserAgent;
                    if (!string.IsNullOrEmpty(userAgent)) {
                        var regEx = new Regex("twiceler|baduspider|slurp|ask|teoma|yahoo|bingbot|googlebot|msnbot|yandexbot|ahrefsbot|paessler|nagios|pingdom");
                        result = regEx.Match(userAgent.ToLower()).Success;
                    }
                }

                if (!context.Request.Browser.Cookies) {
                    logging.Trace("{0}|{1}|{2}|{3}", context.Request.ServerVariables["HTTP_CLIENT_IP"], context.Request.Browser.Crawler, context.Request.RequestType, context.Request.UserAgent);
                }
            }
            catch (Exception exception) {
                logging.Debug(exception);
            }

            return result;
        }

        private bool TryWriteGlobalAsax() {
            try {
                File.SetLastWriteTimeUtc(MapPath("~/global.asax"), DateTime.UtcNow);
                return true;
            }
            catch {
                return false;
            }
        }

        private bool TryWriteWebConfig() {
            try {
                // In medium trust, "UnloadAppDomain" is not supported. Touch web.config
                // to force an AppDomain restart.
                File.SetLastWriteTimeUtc(MapPath("~/web.config"), DateTime.UtcNow);
                return true;
            }
            catch {
                return false;
            }
        }
    }
}
