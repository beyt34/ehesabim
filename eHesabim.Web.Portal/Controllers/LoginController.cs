using System;
using System.Web.Mvc;

using eHesabim.Framework;
using eHesabim.Framework.Localization;
using eHesabim.Services;
using eHesabim.Web.Portal.Engine;
using eHesabim.Web.Portal.Models;

namespace eHesabim.Web.Portal.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService userService;
        private readonly IWorkContext workContext;

        public LoginController(IUserService userService, IWorkContext workContext)
        {
            this.userService = userService;
            this.workContext = workContext;
        }

        public ActionResult Index(string returnUrl)
        {
            if (workContext.CurrentUser != null)
            {
                Response.RedirectToRoute(RouteNames.Home);
            }

            return View(new UserLoginWebModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public ActionResult Index(UserLoginWebModel model)
        {
            if (ModelState.IsValid)
            {
                var userDataModel = userService.GetUserByEmailAndPassword(model.Email.Trim(), model.Password.Trim());
                if (userDataModel != null)
                {
                    workContext.SignIn(userDataModel, model.RememberMe);

                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        Response.Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        Response.RedirectToRoute(RouteNames.Home);
                    }
                }
                else
                {
                    model.ErrorMessage = Messages.LoginError;
                }
            }

            return View(model);
        }

        public ActionResult Register()
        {
            if (workContext.CurrentUser != null)
            {
                Response.RedirectToRoute(RouteNames.Home);
            }

            return View(new UserRegisterWebModel());
        }

        [HttpPost]
        public ActionResult Register(UserRegisterWebModel model)
        {
            if (ModelState.IsValid)
            {
                string errMessage;
                var id = userService.RegisterUser(model.Name, model.Email, model.Password, out errMessage);

                if (string.IsNullOrEmpty(errMessage) && id != 0)
                {
                    var userDataModel = userService.GetUserById(id);
                    workContext.SignIn(userDataModel, false);
                    Response.RedirectToRoute(RouteNames.Home);
                }
                else
                {
                    model.ErrorMessage = Messages.ResourceManager.GetString(errMessage ?? string.Empty);
                }
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            workContext.SignOut();
            return RedirectToRoute(RouteNames.Home);
        }

        public ActionResult PasswordRecovery()
        {
            return View(new UserPasswordRecoveryWebModel());
        }

        [HttpPost]
        public ActionResult PasswordRecovery(UserPasswordRecoveryWebModel model)
        {
            var returnModel = new UserPasswordRecoveryWebModel();
            if (ModelState.IsValid)
            {
                var result = userService.SendPasswordGuid(model.Email) != Guid.Empty;

                if (result)
                {
                    returnModel.SuccessMessage = Messages.SendPasswordGuidSuccess;
                }
                else
                {
                    returnModel.ErrorMessage = Messages.SendPasswordGuidError;
                }

                return View(returnModel);
            }

            return View(returnModel);
        }

        public ActionResult PasswordRecoveryConfirm(string token)
        {
            if (!CheckPasswordRecoveryData(token))
            {
                return RedirectToRoute(RouteNames.Home);
            }

            var model = new UserPasswordRecoveryConfirmWebModel();
            Guid guidPasswordRecoveryToken;
            Guid.TryParse(token, out guidPasswordRecoveryToken);
            model.PasswordGuid = guidPasswordRecoveryToken;

            return View(model);
        }

        [HttpPost]
        public ActionResult PasswordRecoveryConfirm(UserPasswordRecoveryConfirmWebModel model)
        {
            var baseModel = new UserPasswordRecoveryConfirmWebModel();
            if (ModelState.IsValid)
            {
                if (model.PasswordGuid == default(Guid))
                {
                    return RedirectToRoute(RouteNames.Home);
                }

                var userDataModel = userService.SetNewPassword(model.PasswordGuid, model.Password);

                if (userDataModel != null)
                {
                    workContext.SignIn(userDataModel, false);
                    return RedirectToRoute(RouteNames.Home);
                }

                baseModel.ErrorMessage = Messages.PasswordRecoveryConfirmError;
            }

            return View(baseModel);
        }

        private bool CheckPasswordRecoveryData(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            Guid guidPasswordRecoveryToken;
            if (!Guid.TryParse(token, out guidPasswordRecoveryToken))
            {
                return false;
            }

            return userService.CheckPasswordGuid(guidPasswordRecoveryToken);
        }
    }
}