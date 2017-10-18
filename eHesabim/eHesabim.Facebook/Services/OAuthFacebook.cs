using System.Configuration;
using System.IO;
using System.Net;
using System.Web;

using eHesabim.Facebook.GraphApiEntities;

namespace eHesabim.Facebook.Services {
    /// <summary>
    /// Authorization Facebook
    /// </summary>
    public class OAuthFacebook {
        /// <summary>
        /// Authorize Url
        /// </summary>
        public const string Authorize = "https://graph.facebook.com/oauth/authorize";

        /// <summary>
        /// Access Token
        /// </summary>
        public const string AccessToken = "https://graph.facebook.com/oauth/access_token";

        /// <summary>
        /// Callback Url
        /// </summary>
        private readonly string callbackUrl;

        /// <summary>
        /// Application Id
        /// </summary>
        private readonly string applicationId;

        /// <summary>
        /// Application Secret
        /// </summary>
        private readonly string applicationSecret;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthFacebook"/> class.
        /// </summary>
        /// <param name="applicationId"> The application id. </param>
        /// <param name="secretKey"> The secret key. </param>
        public OAuthFacebook(string applicationId, string secretKey) {
            callbackUrl = string.Format("{0}/Login/RegisterFacebook?uid=123&", ConfigurationManager.AppSettings["PortalPath"]);
            this.applicationId = applicationId;
            applicationSecret = secretKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthFacebook"/> class.
        /// </summary>
        /// <param name="token"> The token. </param>
        public OAuthFacebook(string token) {
            Token = token;
        }

        /// <summary>
        /// method enum
        /// </summary>
        public enum Method {
            /// <summary>
            /// Get method
            /// </summary>
            Get,

            /// <summary>
            /// Set method
            /// </summary>
            Post
        }

        /// <summary>
        /// Gets or sets Token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// web request
        /// </summary>
        /// <param name="method">method type</param>
        /// <param name="url">request url</param>
        /// <param name="postData">post data</param>
        /// <param name="userAgent">user agent</param>
        /// <returns>result data</returns>
        public static string WebRequest(Method method, string url, string postData, string userAgent) {
            var responseData = string.Empty;

            var webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            if (webRequest != null) {
                webRequest.Method = method.ToString().ToUpper();
                webRequest.ServicePoint.Expect100Continue = false;
                webRequest.UserAgent = userAgent;
                webRequest.Timeout = 20000;

                if (method == Method.Post) {
                    webRequest.ContentType = "application/x-www-form-urlencoded";

                    // POST the data.
                    var requestWriter = new StreamWriter(webRequest.GetRequestStream());

                    try {
                        requestWriter.Write(postData);
                    }
                    finally {
                        requestWriter.Close();
                    }
                }

                responseData = WebResponseGet(webRequest);
            }

            return responseData;
        }

        /// <summary>
        /// Web response
        /// </summary>
        /// <param name="webRequest">web request</param>
        /// <returns>response data</returns>
        public static string WebResponseGet(HttpWebRequest webRequest) {
            StreamReader responseReader = null;
            var responseData = string.Empty;

            try {
                if (webRequest != null) {
                    var responseStream = webRequest.GetResponse().GetResponseStream();
                    if (responseStream != null) {
                        responseReader = new StreamReader(responseStream);
                    }
                }

                if (responseReader != null) {
                    responseData = responseReader.ReadToEnd();
                }
            }
            finally {
                if (webRequest != null) {
                    var responseStream = webRequest.GetResponse().GetResponseStream();
                    if (responseStream != null) {
                        responseStream.Close();
                    }
                }

                if (responseReader != null) {
                    responseReader.Close();
                }
            }

            return responseData;
        }

        /// <summary>returns authorization link
        /// </summary>
        /// <param name="authorizationOptions"> The authorization options. </param>
        /// <param name="returnUrl"> returnUrl </param>
        /// <returns>authorization link </returns>
        public string AuthorizationLinkGet(AutorizationOptions authorizationOptions, string returnUrl = null) {
            var strExtended = authorizationOptions.GetListOfOptions();
            var ret = string.Format("{0}?client_id={1}&redirect_uri={2}&scope={3}&state={4}", Authorize, applicationId, callbackUrl, strExtended, returnUrl);
            return ret;
        }

        /// <summary>
        /// Returns access token
        /// </summary>
        /// <param name="authToken">access token</param>
        public void AccessTokenGet(string authToken) {
            Token = authToken;
            var accessTokenUrl = string.Format(
                "{0}?client_id={1}&redirect_uri={2}&client_secret={3}&code={4}",
                AccessToken,
                applicationId,
                callbackUrl,
                applicationSecret,
                authToken);

            var response = WebRequest(Method.Get, accessTokenUrl, string.Empty, "YapTapTest");

            if (response.Length > 0) {
                // Store the returned access_token
                var qs = HttpUtility.ParseQueryString(response);

                if (qs["access_token"] != null) {
                    Token = qs["access_token"];
                }
            }
        }
    }
}