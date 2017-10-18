using System;
using System.Web;

using eHesabim.Facebook.GraphApiEntities;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace eHesabim.Facebook.Services {
    /// <summary>
    /// Facebook Service
    /// </summary>
    public class FacebookSvc {
        /// <summary>
        /// Get friend
        /// </summary>
        /// <param name="token"> The token. </param>
        /// <returns>friends list </returns>k
        public static Friends GetFriends(string token) {
            return GetFriends(token, "me");
        }

        /// <summary>
        /// Gets user data
        ///  </summary>
        /// <param name="token"> The token. </param>
        /// <param name="username"> The username. </param>
        /// <returns>person data </returns>
        public static Person GetUserData(string token, string username) {
            const string DataType = "";
            var json1 = GetFacebookData(DataType, token, username);
            var person = JsonConvert.DeserializeObject<Person>(json1);
            var objJson = JObject.Parse(json1);
            person.FirstName = (string)objJson["first_name"];
            person.LastName = (string)objJson["last_name"];
            person.Birthday = !string.IsNullOrEmpty(Convert.ToString(objJson["birthday"])) ?
                DateTime.Parse(Convert.ToString(objJson["birthday"]), System.Globalization.CultureInfo.CreateSpecificCulture("en-us").DateTimeFormat) : 
                DateTime.Now;
            return person;
        }

        /// <summary>
        /// Home items
        /// </summary>
        /// <param name="token"> The token. </param>
        /// <param name="username"> The username. </param>
        /// <returns> wallpost items </returns>
        public static WallPostItems GetHome(string token, string username) {
            const string DataType = "home";  // news feed
            var json = GetFacebookData(DataType, token, username);
            return JsonConvert.DeserializeObject<WallPostItems>(json);
        }

        /// <summary>
        /// gets news feed
        /// </summary>
        /// <param name="token"> The token. </param>
        /// <param name="username"> The username. </param>
        /// <returns>Wallpost items </returns>
        public static WallPostItems GetNewsFeed(string token, string username) {
            const string DataType = "feed"; // profile - wall
            var json = GetFacebookData(DataType, token, username);
            return JsonConvert.DeserializeObject<WallPostItems>(json);
        }

        /// <summary>
        /// gets friend list
        /// </summary>
        /// <param name="token"> The token. </param>
        /// <param name="username"> The username. </param>
        /// <returns>Friend list </returns>
        public static Friends GetFriends(string token, string username) {
            const string DataType = "friends";
            var json = GetFacebookData(DataType, token, username);
            return JsonConvert.DeserializeObject<Friends>(json);
        }

        /// <summary>
        /// Post facebook note
        /// </summary>
        /// <param name="subject"> The subject. </param>
        /// <param name="message"> The message. </param>
        /// <param name="username"> The username. </param>
        /// <param name="token"> The token. </param>
        public static void PostFacebookNote(string subject, string message, string username, string token) {
            const string DataType = "notes";
            var url = string.Format("https://graph.facebook.com/{2}/{0}?access_token={1}", DataType, token, username);
            OAuthFacebook.WebRequest(OAuthFacebook.Method.Post, url, string.Format("message={0}&subject={1}", message, HttpUtility.UrlEncode(subject)), "YapTapTest");
        }

        /// <summary>Test data </summary>
        /// <param name="token">The token. </param>
        public void TestMethodGetAllData(string token) {
            const string Username = "me";
            ////var userData = GetUserData(token, username);
            ////var friends = GetFriends(token, username);
            ////var homeItems = GetHome(token, username);
            ////var newsItems = GetNewsFeed(token, username);
            PostFacebookNote("test subject", "test message", Username, token);
        }

        /// <summary>
        /// Gets facebook data
        /// </summary>
        /// <param name="dataType"> The data type. </param>
        /// <param name="token"> The token. </param>
        /// <param name="username"> The username. </param>
        /// <returns>facebook data</returns>
        private static string GetFacebookData(string dataType, string token, string username) {
            var url = string.Format("https://graph.facebook.com/{2}/{0}?access_token={1}", dataType, token, username);
            return OAuthFacebook.WebRequest(OAuthFacebook.Method.Get, url, string.Empty, "YapTapTest");
        }
    }
}