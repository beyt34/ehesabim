using System;
using System.Web;
using System.Web.Security;

using eHesabim.Services;
using eHesabim.Services.Models;

namespace eHesabim.Framework {
    public class WebWorkContext : IWorkContext {
        private readonly HttpContextBase httpContext;
        private readonly IUserService userService;
        private readonly TimeSpan expirationTimeSpan;
        private UserDataModel cachedUser;

        public WebWorkContext(HttpContextBase httpContext, IUserService userService) {
            this.httpContext = httpContext;
            this.userService = userService;
            expirationTimeSpan = FormsAuthentication.Timeout;
        }

        public UserDataModel CurrentUser {
            get {
                return GetCurrentUser();
            }
        }

        public void SignIn(UserDataModel user, bool createPersistentCookie) {
            var now = DateTime.UtcNow.ToLocalTime();

            var cookieTimeSpan = expirationTimeSpan;
            if (createPersistentCookie) {
                cookieTimeSpan = new TimeSpan(30, 0, 0, 0);
            }

            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                user.Email,
                now,
                now.Add(cookieTimeSpan),
                createPersistentCookie,
                user.Email,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket) {
                HttpOnly = true,
                Expires = now.Add(cookieTimeSpan),
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath
            };

            if (FormsAuthentication.CookieDomain != null) {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            httpContext.Response.Cookies.Add(cookie);
            cachedUser = user;
        }

        public void SignOut() {
            cachedUser = null;
            FormsAuthentication.SignOut();
        }

        private UserDataModel GetCurrentUser() {
            if (cachedUser != null) {
                return cachedUser;
            }

            UserDataModel userDataModel = null;
            if (httpContext != null) {
                // registered account
                userDataModel = GetAuthenticatedUser();
            }

            // validation
            if (userDataModel != null) {
                cachedUser = userDataModel;
            }

            return cachedUser;
        }

        private UserDataModel GetAuthenticatedUser() {
            if (cachedUser != null) {
                return cachedUser;
            }

            if (httpContext == null || httpContext.Request == null || !httpContext.Request.IsAuthenticated || !(httpContext.User.Identity is FormsIdentity)) {
                return null;
            }

            var formsIdentity = (FormsIdentity)httpContext.User.Identity;
            var user = GetAuthenticatedUserFromTicket(formsIdentity.Ticket);
            if (user != null) {
                cachedUser = user;
            }

            return cachedUser;
        }

        private UserDataModel GetAuthenticatedUserFromTicket(FormsAuthenticationTicket ticket) {
            if (ticket == null) {
                throw new ArgumentNullException("ticket");
            }

            var email = ticket.UserData;
            if (string.IsNullOrWhiteSpace(email)) {
                return null;
            }

            return userService.GetUserByEmail(email);
        }
    }
}