using System;
using System.Collections.Generic;
using System.Web.Mvc;
using eHesabim.Framework;
using eHesabim.Services;
using eHesabim.Services.Models;
using eHesabim.Web.Portal.Engine;
using eHesabim.Web.Portal.Models;
using Kendo.Mvc.UI;

namespace eHesabim.Web.Portal.Controllers {
    public class BankController : BaseController {
        private readonly IBankAccountService bankAccountService;
        private readonly IBankCreditService bankCreditService;
        private readonly IBankCreditCardService bankCreditCardService;
        private readonly IExpenseService expenseService;
        private readonly ICommonService commonService;
        private readonly IWorkContext workContext;

        public BankController(IBankAccountService bankAccountService, IBankCreditService bankCreditService, IBankCreditCardService bankCreditCardService, IExpenseService expenseService, ICommonService commonService, IWorkContext workContext) {
            this.bankAccountService = bankAccountService;
            this.bankCreditService = bankCreditService;
            this.bankCreditCardService = bankCreditCardService;
            this.expenseService = expenseService;
            this.commonService = commonService;
            this.workContext = workContext;
        }

        public ActionResult BankCreditList([DataSourceRequest]DataSourceRequest request, BankCreditListWebModel model) {
            if (!Request.IsAjaxRequest()) {
                return View(GetBankCreditListWebModel());
            }

            SetSort(request);
            return Json(GetBankCreditListWebModel(model.SearchBankId, SortField, SortDescending, model.SearchViaForm ? 0 : request.Page - 1, request.PageSize));
        }

        [ActionName("_BankCreditEdit")]
        public PartialViewResult BankCreditEdit(Guid? id) {
            var model = new BankCreditEditWebModel { CreditDateTime = DateTime.Today };

            if ((id ?? Guid.Empty) != Guid.Empty) {
                var dataModel = bankCreditService.GetBankCreditById(id ?? Guid.Empty, workContext.CurrentUser.Id);
                if (dataModel == null) {
                    return PartialView(model);
                }

                model = Engine.AutoMapperConfiguration.Mapper.Map<BankCreditDataModel, BankCreditEditWebModel>(dataModel);
            }

            model.BankList = GetSelectList(commonService.GetBankList());
            return PartialView(model);
        }

        [HttpPost, ValidateInput(false), ActionName("_BankCreditEdit")]
        public ActionResult BankCreditEdit(BankCreditEditWebModel model) {
            if (ModelState.IsValid) {
                string errMessage;
                bankCreditService.AddUpdateBankCredit(
                    model.Id,
                    workContext.CurrentUser.Id,
                    model.BankId,
                    model.CreditDateTime,
                    model.Capital,
                    model.Rate,
                    model.Installment,
                    model.MonthlyPayment,
                    model.Expense,
                    out errMessage);

                return Json(new { Success = string.IsNullOrEmpty(errMessage), Result = errMessage });
            }

            return Json(new { Success = false, Result = Framework.WebViewPage.ValidationSummary(ModelState, string.Empty) });
        }

        public ActionResult BankCreditSubList([DataSourceRequest]DataSourceRequest request, BankCreditSubListWebModel model) {
            if (!Request.IsAjaxRequest()) {
                return View(GetBankCreditSubListWebModel());
            }

            SetSort(request);
            return Json(GetBankCreditSubListWebModel(model.SearchBankCreditId, SortField, SortDescending, model.SearchViaForm ? 0 : request.Page - 1, request.PageSize));
        }

        [ActionName("_BankCreditSubEdit")]
        public PartialViewResult BankCreditSubEdit(Guid? id) {
            var model = new BankCreditSubEditWebModel();

            if ((id ?? Guid.Empty) != Guid.Empty) {
                var dataModel = bankCreditService.GetBankCreditSubById(id ?? Guid.Empty, workContext.CurrentUser.Id);
                if (dataModel == null) {
                    return PartialView(model);
                }

                model = Engine.AutoMapperConfiguration.Mapper.Map<BankCreditSubDataModel, BankCreditSubEditWebModel>(dataModel);
            }

            model.BankCreditList = GetSelectList(bankCreditService.GetBankCreditList(workContext.CurrentUser.Id));
            model.BankAccountList = GetSelectList(bankAccountService.GetBankAccountList(workContext.CurrentUser.Id));
            model.ExpenseGroupList = GetSelectList(expenseService.GetExpenseGroupList(workContext.CurrentUser.Id));

            return PartialView(model);
        }

        [HttpPost, ValidateInput(false), ActionName("_BankCreditSubEdit")]
        public ActionResult BankCreditSubEdit(BankCreditSubEditWebModel model) {
            if (ModelState.IsValid) {
                string errMessage;
                bankCreditService.AddUpdateBankCreditSub(
                    model.Id,
                    workContext.CurrentUser.Id,
                    model.BankCreditId,
                    model.Installment,
                    model.InstallmentDateTime,
                    model.PaymentDateTime,
                    model.InstallmentAmount,
                    model.CapitalAmount,
                    model.InterestAmount,
                    model.BankAccountId,
                    model.ExpenseGroupId,
                    out errMessage);

                return Json(new { Success = string.IsNullOrEmpty(errMessage), Result = errMessage });
            }

            return Json(new { Success = false, Result = Framework.WebViewPage.ValidationSummary(ModelState, string.Empty) });
        }

        [HttpPost]
        public ActionResult GetPaymentPlan(BankCreditEditWebModel model) {
            // todo : get calculation
            ////var rate = Convert.ToDouble(model.Rate / 100.0M * 1.2M);
            ////var capital = Convert.ToDouble(model.Capital);
            ////var payment = Financial.Pmt(rate, model.Installment, capital) * (-1);

            var list = new List<BankCreditSubEditWebModel>();
            for (var i = 1; i <= model.Installment; i++) {
                var item = new BankCreditSubEditWebModel {
                    Id = Guid.Empty,
                    Installment = i,
                    InstallmentDateTime = model.CreditDateTime.AddMonths(i),
                    InstallmentAmount = model.MonthlyPayment
                };
                list.Add(item);
            }

            return Json(new {
                Result = true,
                PartialHtml = ControllerContextHelper.RenderRazorViewToString(ControllerContext, "~/Views/Bank/_BankCreditSubEdit.cshtml", list),
            });
        }

        public ActionResult BankCreditCardList([DataSourceRequest]DataSourceRequest request, BankCreditCardListWebModel model) {
            if (!Request.IsAjaxRequest()) {
                return View(GetBankCreditCardListWebModel());
            }

            SetSort(request);
            return Json(GetBankCreditCardListWebModel(model.SearchBankId, SortField, SortDescending, model.SearchViaForm ? 0 : request.Page - 1, request.PageSize));
        }

        [ActionName("_BankCreditCardEdit")]
        public PartialViewResult BankCreditCardEdit(Guid? id) {
            var model = new BankCreditCardEditWebModel { IsActive = true };

            if ((id ?? Guid.Empty) != Guid.Empty) {
                var dataModel = bankCreditCardService.GetBankCreditCardById(id ?? Guid.Empty, workContext.CurrentUser.Id);
                if (dataModel == null) {
                    return PartialView(model);
                }

                model = Engine.AutoMapperConfiguration.Mapper.Map<BankCreditCardDataModel, BankCreditCardEditWebModel>(dataModel);
            }

            model.ParentList = GetSelectList(bankCreditCardService.GetBankCreditCardList(workContext.CurrentUser.Id, id, null));
            model.BankList = GetSelectList(commonService.GetBankList());
            return PartialView(model);
        }

        [HttpPost, ValidateInput(false), ActionName("_BankCreditCardEdit")]
        public ActionResult BankCreditCardEdit(BankCreditCardEditWebModel model) {
            if ((model.ParentId ?? Guid.Empty) != Guid.Empty) {
                model.BankId = bankCreditCardService.GetBankId(model.ParentId);
                if (model.BankId > 0) {
                    ModelState.Remove("BankId");
                }
            }

            if (ModelState.IsValid) {
                string errMessage;
                bankCreditCardService.AddUpdateBankCreditCard(
                    model.Id,
                    workContext.CurrentUser.Id,
                    model.ParentId,
                    model.BankId,
                    model.Name,
                    model.CardNo,
                    model.Limit,
                    model.ExpiredMonth,
                    model.ExpiredYear,
                    model.Order,
                    model.IsActive,
                    out errMessage);

                return Json(new { Success = string.IsNullOrEmpty(errMessage), Result = errMessage });
            }

            return Json(new { Success = false, Result = Framework.WebViewPage.ValidationSummary(ModelState, string.Empty) });
        }

        [HttpPost]
        public ActionResult GetBankId(Guid? parentId) {
            var bankId = bankCreditCardService.GetBankId(parentId);
            return Json(bankId);
        }

        public ActionResult BankCreditCardPeriodList([DataSourceRequest]DataSourceRequest request, BankCreditCardPeriodListWebModel model) {
            if (!Request.IsAjaxRequest()) {
                return View(GetBankCreditCardPeriodListWebModel());
            }

            SetSort(request);
            return Json(GetBankCreditCardPeriodListWebModel(model.SearchBankCreditCardId, SortField, SortDescending, model.SearchViaForm ? 0 : request.Page - 1, request.PageSize));
        }

        [ActionName("_BankCreditCardPeriodEdit")]
        public PartialViewResult BankCreditCardPeriodEdit(Guid? id, Guid? creditCardId) {
            var model = new BankCreditCardPeriodEditWebModel {
                StartDate = DateTime.Today,
                BankCreditCardId = creditCardId ?? default(Guid),
                SetExpense = true,
            };

            if ((id ?? Guid.Empty) != Guid.Empty) {
                var dataModel = bankCreditCardService.GetBankCreditCardPeriodById(id ?? Guid.Empty, workContext.CurrentUser.Id);
                if (dataModel == null) {
                    return PartialView(model);
                }

                model = Engine.AutoMapperConfiguration.Mapper.Map<BankCreditCardPeriodDataModel, BankCreditCardPeriodEditWebModel>(dataModel);
            }

            model.BankCreditCardList = GetSelectList(bankCreditCardService.GetBankCreditCardList(workContext.CurrentUser.Id, null, null));
            return PartialView(model);
        }

        [HttpPost, ValidateInput(false), ActionName("_BankCreditCardPeriodEdit")]
        public ActionResult BankCreditCardPeriodEdit(BankCreditCardPeriodEditWebModel model) {
            if (ModelState.IsValid) {
                string errMessage;
                bankCreditCardService.AddUpdateBankCreditCardPeriod(
                    model.Id,
                    workContext.CurrentUser.Id,
                    model.BankCreditCardId,
                    model.StartDate,
                    model.EndDate,
                    model.PaymentDate,
                    model.SetExpense,
                    out errMessage);

                return Json(new { Success = string.IsNullOrEmpty(errMessage), Result = errMessage });
            }

            return Json(new { Success = false, Result = Framework.WebViewPage.ValidationSummary(ModelState, string.Empty) });
        }

        [HttpPost]
        public ActionResult SetExpensePeriod(Guid id) {
            bankCreditCardService.SetExpensePeriod(id, workContext.CurrentUser.Id);
            return Json(new { Success = true, Message = Framework.Localization.Messages.SetPeriodSuccesfully });
        }

        public ActionResult BankCreditCardPaymentList([DataSourceRequest]DataSourceRequest request, BankCreditCardPaymentListWebModel model) {
            if (!Request.IsAjaxRequest()) {
                return View(GetBankCreditCardPaymentListWebModel());
            }

            SetSort(request);
            return Json(GetBankCreditCardPaymentListWebModel(model.SearchBankCreditCardId, SortField, SortDescending, model.SearchViaForm ? 0 : request.Page - 1, request.PageSize));
        }

        [ActionName("_BankCreditCardPaymentEdit")]
        public PartialViewResult BankCreditCardPaymentEdit(Guid? id, Guid? creditCardId) {
            var model = new BankCreditCardPaymentEditWebModel {
                PaymentDateTime = DateTime.Today,
                BankCreditCardId = creditCardId ?? default(Guid),
            };

            if ((id ?? Guid.Empty) != Guid.Empty) {
                var dataModel = bankCreditCardService.GetBankCreditCardPaymentById(id ?? Guid.Empty, workContext.CurrentUser.Id);
                if (dataModel == null) {
                    return PartialView(model);
                }

                model = Engine.AutoMapperConfiguration.Mapper.Map<BankCreditCardPaymentDataModel, BankCreditCardPaymentEditWebModel>(dataModel);
            }

            model.BankCreditCardList = GetSelectList(bankCreditCardService.GetBankCreditCardList(workContext.CurrentUser.Id, null, null));
            model.BankAccountList = GetSelectList(bankAccountService.GetBankAccountList(workContext.CurrentUser.Id));

            return PartialView(model);
        }

        [HttpPost, ValidateInput(false), ActionName("_BankCreditCardPaymentEdit")]
        public ActionResult BankCreditCardPaymentEdit(BankCreditCardPaymentEditWebModel model) {
            if (ModelState.IsValid) {
                string errMessage;
                bankCreditCardService.AddUpdateBankCreditCardPayment(
                    model.Id,
                    workContext.CurrentUser.Id,
                    model.BankCreditCardId,
                    model.PaymentDateTime,
                    model.Amount,
                    model.HasPoint,
                    model.BankAccountId,
                    out errMessage);

                return Json(new { Success = string.IsNullOrEmpty(errMessage), Result = errMessage });
            }

            return Json(new { Success = false, Result = Framework.WebViewPage.ValidationSummary(ModelState, string.Empty) });
        }

        private BankCreditListWebModel GetBankCreditListWebModel(int bankId = 0, string sort = "", bool sortDescending = false, int? pageIndex = 0, int? pageSize = 20) {
            int total;
            var data = bankCreditService.GetBankCreditList(workContext.CurrentUser.Id, bankId, sort, sortDescending, pageIndex ?? 0, pageSize ?? 100, out total);
            var model = Engine.AutoMapperConfiguration.Mapper.Map<List<BankCreditDataModel>, List<BankCreditWebModel>>(data);

            return new BankCreditListWebModel {
                SearchBankList = GetSelectList(commonService.GetBankList()),
                Data = model,
                DeleteData = new DeleteWebModel { Permission = PermissionFormEnum.BankCredit, Form = FormEnum.BankCredit, GridName = "bankCreditGrid" },
                Total = total
            };
        }

        private BankCreditSubListWebModel GetBankCreditSubListWebModel(Guid? bankCreditId = null, string sort = "", bool sortDescending = false, int? pageIndex = 0, int? pageSize = 20) {
            int total;
            var data = bankCreditService.GetBankCreditSubList(workContext.CurrentUser.Id, bankCreditId, sort, sortDescending, pageIndex ?? 0, pageSize ?? 100, out total);
            var model = Engine.AutoMapperConfiguration.Mapper.Map<List<BankCreditSubDataModel>, List<BankCreditSubWebModel>>(data);

            return new BankCreditSubListWebModel {
                SearchBankCreditList = GetSelectList(bankCreditService.GetBankCreditList(workContext.CurrentUser.Id)),
                Data = model,
                DeleteData = new DeleteWebModel { Permission = PermissionFormEnum.BankCredit, Form = FormEnum.BankCreditSub, GridName = "bankCreditSubGrid" },
                Total = total
            };
        }

        private BankCreditCardListWebModel GetBankCreditCardListWebModel(int bankId = 0, string sort = "", bool sortDescending = false, int? pageIndex = 0, int? pageSize = 20) {
            int total;
            var data = bankCreditCardService.GetBankCreditCardList(workContext.CurrentUser.Id, bankId, sort, sortDescending, pageIndex ?? 0, pageSize ?? 100, out total);
            var model = Engine.AutoMapperConfiguration.Mapper.Map<List<BankCreditCardDataModel>, List<BankCreditCardWebModel>>(data);

            return new BankCreditCardListWebModel {
                SearchBankId = bankId,
                SearchBankList = GetSelectList(commonService.GetBankList()),
                Data = model,
                DeleteData = new DeleteWebModel { Permission = PermissionFormEnum.BankCreditCard, Form = FormEnum.BankCreditCard, GridName = "bankCreditCardGrid" },
                Total = total
            };
        }

        private BankCreditCardPeriodListWebModel GetBankCreditCardPeriodListWebModel(Guid? creditCardId = null, string sort = "", bool sortDescending = false, int? pageIndex = 0, int? pageSize = 20) {
            int total;
            var data = bankCreditCardService.GetBankCreditCardPeriodList(workContext.CurrentUser.Id, creditCardId, sort, sortDescending, pageIndex ?? 0, pageSize ?? 100, out total);
            var model = Engine.AutoMapperConfiguration.Mapper.Map<List<BankCreditCardPeriodDataModel>, List<BankCreditCardPeriodWebModel>>(data);

            return new BankCreditCardPeriodListWebModel {
                SearchBankCreditCardId = creditCardId,
                SearchBankCreditCardList = GetSelectList(bankCreditCardService.GetBankCreditCardList(workContext.CurrentUser.Id, null, null)),
                Data = model,
                DeleteData = new DeleteWebModel { Permission = PermissionFormEnum.BankCreditCard, Form = FormEnum.BankCreditCardPeriod, GridName = "bankCreditCardPeriodGrid" },
                Total = total
            };
        }

        private BankCreditCardPaymentListWebModel GetBankCreditCardPaymentListWebModel(Guid? creditCardId = null, string sort = "", bool sortDescending = false, int? pageIndex = 0, int? pageSize = 20) {
            int total;
            var data = bankCreditCardService.GetBankCreditCardPaymentList(workContext.CurrentUser.Id, creditCardId, sort, sortDescending, pageIndex ?? 0, pageSize ?? 100, out total);
            var model = Engine.AutoMapperConfiguration.Mapper.Map<List<BankCreditCardPaymentDataModel>, List<BankCreditCardPaymentWebModel>>(data);

            return new BankCreditCardPaymentListWebModel {
                SearchBankCreditCardId = creditCardId,
                SearchBankCreditCardList = GetSelectList(bankCreditCardService.GetBankCreditCardList(workContext.CurrentUser.Id, null, null)),
                Data = model,
                DeleteData = new DeleteWebModel { Permission = PermissionFormEnum.BankCreditCard, Form = FormEnum.BankCreditCardPayment, GridName = "bankCreditCardPaymentGrid" },
                Total = total
            };
        }
    }
}