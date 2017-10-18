using System.Web;

namespace eHesabim.Core {
    public interface IWebHelper {
        string GetUrlReferrer();

        string GetCurrentIpAddress();

        string GetThisPageUrl(bool includeQueryString);

        string GetThisPageUrl(bool includeQueryString, bool useSsl);

        bool IsCurrentConnectionSecured();

        bool SslEnabled();

        string ServerVariables(string name);

        string GetStoreHost(bool useSsl);

        string GetStoreLocation();

        string GetStoreLocation(bool useSsl);

        bool IsStaticResource(HttpRequest request);

        string MapPath(string path);

        string ModifyQueryString(string url, string queryStringModification, string targetLocationModification);

        string RemoveQueryString(string url, string queryString);

        T QueryString<T>(string name);

        void RestartAppDomain(string redirectUrl = "");

        bool IsSearchEngine(HttpContextBase context);
    }
}
