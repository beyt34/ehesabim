using System;
using System.Web.Mvc;
using eHesabim.Framework;
using eHesabim.Framework.Controllers;
using eHesabim.Framework.Facebook;
using eHesabim.Framework.Localization;
using eHesabim.Services;
using eHesabim.Web.Portal.Engine;
using eHesabim.Web.Portal.Models;

namespace eHesabim.Web.Portal.Controllers {
    public class LoginController : Controller {
        private readonly IUserService userService;
        private readonly IFacebookService facebookService;
        private readonly IWorkContext workContext;

        public LoginController(IUserService userService, IFacebookService facebookService, IWorkContext workContext) {
            this.userService = userService;
            this.facebookService = facebookService;
            this.workContext = workContext;
        }

        public ActionResult Index(string returnUrl) {
            if (workContext.CurrentUser != null) {
                Response.RedirectToRoute(RouteNames.Home);
            }

            return View(new UserLoginWebModel { LoginReturnUrl = returnUrl });
        }

        [HttpPost, ActionName("Index"), FormValueRequired("login")]
        public ActionResult Index(UserLoginWebModel model) {
            ModelState.Remove("RegisterName");
            ModelState.Remove("RegisterEmail");
            ModelState.Remove("RegisterPassword");
            ModelState.Remove("RegisterRePassword");

            if (ModelState.IsValid) {
                var userDataModel = userService.GetUserByEmailAndPassword(model.LoginEmail.Trim(), model.LoginPassword.Trim());
                if (userDataModel != null) {
                    workContext.SignIn(userDataModel, model.LoginRememberMe);

                    if (!string.IsNullOrEmpty(model.LoginReturnUrl)) {
                        Response.Redirect(model.LoginReturnUrl);
                    }
                    else {
                        Response.RedirectToRoute(RouteNames.Home);
                    }
                }
                else {
                    model.LoginErrorMessage = Messages.LoginError;
                }
            }

            return View(model);
        }

        [HttpPost, ActionName("Index"), FormValueRequired("register")]
        public ActionResult Register(UserLoginWebModel model) {
            ModelState.Remove("LoginEmail");
            ModelState.Remove("LoginPassword");

            if (ModelState.IsValid) {
                string errMessage;
                var id = userService.RegisterUser(model.RegisterName, model.RegisterEmail, model.RegisterPassword, out errMessage);

                if (string.IsNullOrEmpty(errMessage) && id != 0) {
                    var userDataModel = userService.GetUserById(id);
                    workContext.SignIn(userDataModel, false);
                    Response.RedirectToRoute(RouteNames.Home);
                }
                else {
                    model.RegisterErrorMessage = Messages.ResourceManager.GetString(errMessage ?? string.Empty);
                }
            }

            return View(model);
        }

        public ActionResult RegisterFacebook() {
            if (Request["code"] == null) {
                // Redirect the user back to Facebook for authorization.
                return new RedirectResult(facebookService.GetLoginUrl());
            }

            // Get the access token and secret.
            var accessToken = facebookService.GetAccessToken(Request["code"]);

            if (!string.IsNullOrEmpty(accessToken)) {
                // get user info
                var person = facebookService.GetUserInfo(accessToken);
                if (person != null) {
                    // register
                    string errMessage;
                    var id = userService.RegisterFacebook(person.FullName, person.Email, person.Id, out errMessage);

                    if (string.IsNullOrEmpty(errMessage) && id != 0) {
                        var userDataModel = userService.GetUserById(id);
                        workContext.SignIn(userDataModel, false);
                        return RedirectToRoute(RouteNames.Home);
                    }
                }
            }

            return RedirectToRoute(RouteNames.Login);
        }
        
        public ActionResult Logout() {
            workContext.SignOut();
            return RedirectToRoute(RouteNames.Home);
        }

        public ActionResult PasswordRecovery() {
            return View(new UserPasswordRecoveryWebModel());
        }

        [HttpPost]
        public ActionResult PasswordRecovery(UserPasswordRecoveryWebModel model) {
            var returnModel = new UserPasswordRecoveryWebModel();
            if (ModelState.IsValid) {
                var result = userService.SendPasswordGuid(model.Email) != Guid.Empty;

                if (result) {
                    returnModel.SuccessMessage = Messages.SendPasswordGuidSuccess;
                }
                else {
                    returnModel.ErrorMessage = Messages.SendPasswordGuidError;
                }

                return View(returnModel);
            }

            return View(returnModel);
        }

        public ActionResult PasswordRecoveryConfirm(string token) {
            if (!CheckPasswordRecoveryData(token)) {
                return RedirectToRoute(RouteNames.Home);
            }

            var model = new UserPasswordRecoveryConfirmWebModel();
            Guid guidPasswordRecoveryToken;
            Guid.TryParse(token, out guidPasswordRecoveryToken);
            model.PasswordGuid = guidPasswordRecoveryToken;

            return View(model);
        }

        [HttpPost]
        public ActionResult PasswordRecoveryConfirm(UserPasswordRecoveryConfirmWebModel model) {
            var baseModel = new UserPasswordRecoveryConfirmWebModel();
            if (ModelState.IsValid) {
                if (model.PasswordGuid == default(Guid)) {
                    return RedirectToRoute(RouteNames.Home);
                }

                var userDataModel = userService.SetNewPassword(model.PasswordGuid, model.Password);

                if (userDataModel != null) {
                    workContext.SignIn(userDataModel, false);
                    return RedirectToRoute(RouteNames.Home);
                }

                baseModel.ErrorMessage = Messages.PasswordRecoveryConfirmError;
            }

            return View(baseModel);
        }

        private bool CheckPasswordRecoveryData(string token) {
            if (string.IsNullOrEmpty(token)) {
                return false;
            }

            Guid guidPasswordRecoveryToken;
            if (!Guid.TryParse(token, out guidPasswordRecoveryToken)) {
                return false;
            }

            return userService.CheckPasswordGuid(guidPasswordRecoveryToken);
        }
    }
}