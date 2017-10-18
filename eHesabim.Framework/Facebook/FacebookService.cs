using System.Configuration;

using Facebook;

using Newtonsoft.Json;

namespace eHesabim.Framework.Facebook {
    public class FacebookService : IFacebookService {
        private readonly string applicationId;
        private readonly string secretKey;
        private readonly string callbackUrl;
        private readonly string scope;

        public FacebookService() {
            applicationId = ConfigurationManager.AppSettings["FacebookAppId"];
            secretKey = ConfigurationManager.AppSettings["FacebookSecretKey"];
            callbackUrl = ConfigurationManager.AppSettings["FacebookCallbackUrl"];
            scope = "email";
        }

        public string GetLoginUrl() {
            var client = new FacebookClient();
            return client.GetLoginUrl(new { client_id = applicationId, client_secret = secretKey, redirect_uri = callbackUrl, response_type = "code", scope }).ToString();
        }

        public string GetAccessToken(string code) {
            var client = new FacebookClient();
            dynamic result = client.Post("oauth/access_token", new { client_id = applicationId, client_secret = secretKey, redirect_uri = callbackUrl, code });
            return result.access_token.ToString();
        }

        public Person GetUserInfo(string accessToken) {
            var client = new FacebookClient(accessToken);
            ////var person = new Person();

            var me = client.Get("/me?fields=id, email, name").ToString();
            var person = JsonConvert.DeserializeObject<Person>(me);

            ////person.Id = me.id;
            ////person.Email = me.email;
            ////person.FullName = me.name;

            return person;
        }
    }
}
