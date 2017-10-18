using System.Collections.Generic;
using System.Web.Mvc;
using eHesabim.Framework;
using eHesabim.Framework.Localization;
using eHesabim.Services;
using eHesabim.Services.Models;
using eHesabim.Web.Portal.Engine;
using eHesabim.Web.Portal.Models;
using Kendo.Mvc.UI;

namespace eHesabim.Web.Portal.Controllers {
    public class UserController : BaseController {
        private readonly IUserService userService;

        private readonly IWorkContext workContext;

        public UserController(IUserService userService, IWorkContext workContext) {
            this.userService = userService;
            this.workContext = workContext;
        }

        [ActionAuthorize(PermissionTypeEnum.Admin)]
        public ActionResult UserList([DataSourceRequest]DataSourceRequest request, UserListWebModel model) {
            if (!Request.IsAjaxRequest()) {
                return View(GetUserListWebModel());
            }

            SetSort(request);
            return Json(GetUserListWebModel(
                model.SearchName,
                model.SearchEmail,
                SortField,
                SortDescending,
                model.SearchViaForm ? 0 : request.Page - 1,
                request.PageSize));
        }

        [ActionAuthorize(PermissionTypeEnum.Admin)]
        [ActionName("_UserEdit")]
        public ActionResult UserEdit(int? id) {
            var model = new UserEditWebModel();
            if (id > 0) {
                var dataModel = userService.GetUserById((int)id);
                if (dataModel == null) {
                    return RedirectToAction("UserList");
                }

                model = new UserEditWebModel {
                    Id = dataModel.Id,
                    Name = dataModel.Name,
                    Email = dataModel.Email,
                    Phone = dataModel.Phone,
                };
            }

            return PartialView(model);
        }

        [HttpPost, ValidateInput(false), ActionName("_UserEdit")]
        [ActionAuthorize(PermissionTypeEnum.Admin)]
        public ActionResult UserEdit(UserEditWebModel model) {
            if (ModelState.IsValid) {
                string errMessage;
                userService.UpdateUser(model.Id, model.Name, model.Email, model.Password, model.Phone, out errMessage);

                return Json(new { Success = string.IsNullOrEmpty(errMessage), Result = errMessage });
            }

            return Json(new { Success = false, Result = Framework.WebViewPage.ValidationSummary(ModelState, string.Empty) });
        }

        [ActionName("_UpdateProfile")]
        public PartialViewResult UpdateProfile(int userId) {
            var model = new UserUpdateProfileWebModel();

            if (userId != 0) {
                var dataModel = userService.GetUserById(userId);
                if (dataModel == null) {
                    return PartialView(model);
                }

                model = Engine.AutoMapperConfiguration.Mapper.Map<UserDataModel, UserUpdateProfileWebModel>(dataModel);
            }

            return PartialView(model);
        }

        [HttpPost, ValidateInput(false), ActionName("_UpdateProfile")]
        public ActionResult UpdateProfile(UserUpdateProfileWebModel model) {
            if (ModelState.IsValid) {
                string errMessage;
                userService.UpdateUser(model.Id, model.Name, model.Email, model.Phone, model.ShowCustomer, model.ShowAccount, model.ShowCredit, model.ShowCard, model.ShowExpense, model.ShowCustomerExclusion, out errMessage);

                // logout, login
                var user = userService.GetUserById(model.Id);
                workContext.SignOut();
                workContext.SignIn(user, false);

                return Json(new { Success = string.IsNullOrEmpty(errMessage), Result = errMessage });
            }

            return Json(new { Success = false, Result = Framework.WebViewPage.ValidationSummary(ModelState, string.Empty) });
        }

        [ActionName("_PasswordChange")]
        public PartialViewResult PasswordChange(int userId) {
            return PartialView(new UserPasswordChangeWebModel { Id = userId });
        }

        [HttpPost, ValidateInput(false), ActionName("_PasswordChange")]
        public ActionResult PasswordChange(UserPasswordChangeWebModel model) {
            var result = false;
            var message = ModelState.IsValid
                ? string.Empty
                : Framework.WebViewPage.ValidationSummary(ModelState, Messages.Error);

            if (ModelState.IsValid) {
                if (workContext != null && workContext.CurrentUser != null && !string.IsNullOrEmpty(model.Password) && !string.IsNullOrEmpty(model.RePassword)) {
                    result = userService.SetNewPassword(model.Id, model.Password);
                    message = Messages.PasswordChanged;
                }
            }

            return Json(new { Success = result, Result = message });
        }
        
        [HttpPost]
        public ActionResult LoginAs(int id) {
            var user = userService.GetUserById(id);
            if (user != null) {
                workContext.SignIn(user, false);
                return Json(new { Success = true });
            }

            return Json(new { Success = false });
        }

        private UserListWebModel GetUserListWebModel(string name = "", string email = "", string sort = "", bool sortDescending = false, int? pageIndex = 0, int? pageSize = 10) {
            int total;
            var data = userService.GetUserList(name, email, sort, sortDescending, pageIndex ?? 0, pageSize ?? 100, out total);
            return new UserListWebModel {
                Data = Engine.AutoMapperConfiguration.Mapper.Map<List<UserDataModel>, List<UserWebModel>>(data),
                DeleteData = new DeleteWebModel { Permission = PermissionFormEnum.User, Form = FormEnum.User, GridName = "userGrid" },
                Total = total
            };
        }
    }
}