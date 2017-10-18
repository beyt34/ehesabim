using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using eHesabim.Framework;
using eHesabim.Services;
using eHesabim.Services.Models;
using eHesabim.Web.Portal.Models;
using Kendo.Mvc.UI;

namespace eHesabim.Web.Portal.Controllers {
    public class BankAccountController : BaseController {
        private readonly IBankAccountService bankAccountService;
        private readonly ICommonService commonService;
        private readonly IWorkContext workContext;

        public BankAccountController(IBankAccountService bankAccountService, ICommonService commonService, IWorkContext workContext) {
            this.bankAccountService = bankAccountService;
            this.commonService = commonService;
            this.workContext = workContext;
        }

        public ActionResult BankAccountList([DataSourceRequest]DataSourceRequest request, BankAccountListWebModel model) {
            if (!Request.IsAjaxRequest()) {
                return View(GetBankAccountListWebModel());
            }

            SetSort(request);
            return Json(GetBankAccountListWebModel(model.SearchTypeId, model.SearchBankId, SortField, SortDescending, model.SearchViaForm ? 0 : request.Page - 1, request.PageSize));
        }

        [ActionName("_BankAccountEdit")]
        public PartialViewResult BankAccountEdit(Guid? id) {
            var model = new BankAccountEditWebModel { IsActive = true };

            if ((id ?? Guid.Empty) != Guid.Empty) {
                var dataModel = bankAccountService.GetBankAccountById(id ?? Guid.Empty, workContext.CurrentUser.Id);
                if (dataModel == null) {
                    return PartialView(model);
                }

                model = Engine.AutoMapperConfiguration.Mapper.Map<BankAccountDataModel, BankAccountEditWebModel>(dataModel);
            }

            model.TypeList = GetSelectList(commonService.GetTypeList((int)AccountTypeEnum.Parent));
            model.BankList = GetSelectList(commonService.GetBankList());

            return PartialView(model);
        }

        [HttpPost, ValidateInput(false), ActionName("_BankAccountEdit")]
        public ActionResult BankAccountEdit(BankAccountEditWebModel model) {
            if (ModelState.IsValid) {
                string errMessage;
                bankAccountService.AddUpdateBankAccount(
                    model.Id,
                    workContext.CurrentUser.Id,
                    model.TypeId,
                    model.Name,
                    model.BankId,
                    model.Iban,
                    model.Limit,
                    model.IsActive,
                    out errMessage);

                return Json(new { Success = string.IsNullOrEmpty(errMessage), Result = errMessage });
            }

            return Json(new { Success = false, Result = Framework.WebViewPage.ValidationSummary(ModelState, string.Empty) });
        }

        public ActionResult BankAccountTransactionList([DataSourceRequest]DataSourceRequest request, BankAccountTransactionListWebModel model) {
            if (!Request.IsAjaxRequest()) {
                return View(GetBankAccountTransactionListWebModel());
            }

            SetSort(request);
            return Json(
                    GetBankAccountTransactionListWebModel(
                        model.SearchBankAccountId,
                        model.SearchName,
                        model.SearchStartDate,
                        model.SearchEndDate,
                        SortField,
                        SortDescending,
                        model.SearchViaForm ? 0 : request.Page - 1,
                        request.PageSize));
        }

        [ActionName("_BankAccountTransactionEdit")]
        public PartialViewResult BankAccountTransactionEdit(Guid? id, Guid? bankAccountId) {
            var model = new BankAccountTransactionEditWebModel {
                BankAccountId = bankAccountId ?? default(Guid),
                TrnDateTime = DateTime.Today
            };

            if ((id ?? Guid.Empty) != Guid.Empty) {
                var dataModel = bankAccountService.GetBankAccountTransactionById(id ?? Guid.Empty, workContext.CurrentUser.Id);
                if (dataModel == null) {
                    return PartialView(model);
                }

                model = Engine.AutoMapperConfiguration.Mapper.Map<BankAccountTransactionDataModel, BankAccountTransactionEditWebModel>(dataModel);
            }

            model.BankAccountList = GetSelectList(bankAccountService.GetBankAccountList(workContext.CurrentUser.Id));
            model.TypeList = GetSelectList(commonService.GetTypeList((int)BankAccountTransactionTypeEnum.Parent));
            model.RelatedBankAccountList = GetSelectList(bankAccountService.GetBankAccountList(workContext.CurrentUser.Id));

            return PartialView(model);
        }

        [HttpPost, ValidateInput(false), ActionName("_BankAccountTransactionEdit")]
        public ActionResult BankAccountTransactionEdit(BankAccountTransactionEditWebModel model) {
            if (ModelState.IsValid) {
                string errMessage;
                bankAccountService.AddUpdateBankAccountTransaction(
                    model.Id,
                    workContext.CurrentUser.Id,
                    model.BankAccountId,
                    model.TrnDateTime,
                    model.Name,
                    model.TypeId,
                    model.Amount,
                    model.RelatedBankAccountId,
                    out errMessage);

                return Json(new { Success = string.IsNullOrEmpty(errMessage), Result = errMessage });
            }

            return Json(new { Success = false, Result = Framework.WebViewPage.ValidationSummary(ModelState, string.Empty) });
        }

        private BankAccountListWebModel GetBankAccountListWebModel(int typeId = 0, int bankId = 0, string sort = "", bool sortDescending = false, int? pageIndex = 0, int? pageSize = 20) {
            int total;
            var data = bankAccountService.GetBankAccountList(workContext.CurrentUser.Id, typeId, bankId, sort, sortDescending, pageIndex ?? 0, pageSize ?? 100, out total);

            var model = Engine.AutoMapperConfiguration.Mapper.Map<List<BankAccountDataModel>, List<BankAccountWebModel>>(data);

            return new BankAccountListWebModel {
                SearchTypeId = typeId,
                SearchTypeList = GetSelectList(commonService.GetTypeList((int)AccountTypeEnum.Parent)),
                SearchBankId = bankId,
                SearchBankList = GetSelectList(commonService.GetBankList()),
                Data = model,
                DeleteData = new DeleteWebModel { Permission = PermissionFormEnum.BankAccount, Form = FormEnum.BankAccount, GridName = "bankAccountGrid" },
                Total = total
            };
        }

        private BankAccountTransactionListWebModel GetBankAccountTransactionListWebModel(Guid? bankAccountId = null, string name = "", DateTime? startDate = null, DateTime? endDate = null, string sort = "", bool sortDescending = false, int? pageIndex = 0, int? pageSize = 20) {
            int total;
            decimal debtTotal;
            decimal claimTotal;

            var data = bankAccountService.GetBankAccountTransactionList(
                workContext.CurrentUser.Id,
                bankAccountId,
                name,
                startDate,
                endDate,
                sort,
                sortDescending,
                pageIndex ?? 0,
                pageSize ?? 100,
                out total,
                out debtTotal,
                out claimTotal);

            var model = Engine.AutoMapperConfiguration.Mapper.Map<List<BankAccountTransactionDataModel>, List<BankAccountTransactionWebModel>>(data);
            if (model.Any()) {
                var firstItem = model.FirstOrDefault();
                if (firstItem != null) {
                    firstItem.DebitTotal = debtTotal.ToString("N2");
                    firstItem.ClaimTotal = claimTotal.ToString("N2");
                    firstItem.NetTotal = (debtTotal - claimTotal).ToString("N2");
                }
            }

            return new BankAccountTransactionListWebModel {
                SearchBankAccountId = bankAccountId,
                SearchBankAccountList = GetSelectList(bankAccountService.GetBankAccountList(workContext.CurrentUser.Id)),
                SearchStartDate = startDate,
                SearchEndDate = endDate,
                SearchName = name,
                Data = model,
                DeleteData = new DeleteWebModel { Permission = PermissionFormEnum.BankAccount, Form = FormEnum.BankAccountTransaction, GridName = "bankAccountTransactionGrid" },
                Total = total
            };
        }
    }
}