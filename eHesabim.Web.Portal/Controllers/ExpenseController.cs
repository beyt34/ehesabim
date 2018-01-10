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
    public class ExpenseController : BaseController {
        private readonly IBankAccountService bankAccountService;
        private readonly ICommonService commonService;
        private readonly IBankCreditCardService bankCreditCardService;
        private readonly IExpenseService expenseService;
        private readonly IWorkContext workContext;

        public ExpenseController(IBankAccountService bankAccountService, ICommonService commonService, IBankCreditCardService bankCreditCardService, IExpenseService expenseService, IWorkContext workContext) {
            this.bankAccountService = bankAccountService;
            this.commonService = commonService;
            this.bankCreditCardService = bankCreditCardService;
            this.expenseService = expenseService;
            this.workContext = workContext;
        }

        public ActionResult ExpenseList([DataSourceRequest]DataSourceRequest request, ExpenseListWebModel model, Guid? groupId, int? year, int? month) {
            if (!Request.IsAjaxRequest()) {
                var startDate = year.HasValue && month.HasValue ? new DateTime(year.Value, month.Value, 1) : (DateTime?)null;
                var endDate = year.HasValue && month.HasValue ? new DateTime(year.Value, month.Value, 1) : (DateTime?)null;
                var excludeId = 0;
                if (endDate.HasValue) {
                    endDate = endDate.Value.AddMonths(1).AddDays(-1);
                    excludeId = 2;
                }

                return View(GetExpenseListWebModel(groupId: groupId, startDate: startDate, endDate: endDate, excludeId: excludeId));
            }

            SetSort(request);
            return
                Json(
                    GetExpenseListWebModel(
                        model.SearchTypeId,
                        model.SearchExpenseStoreId,
                        model.SearchExpenseGroupId,
                        model.SearchBankCreditCardId,
                        model.SearchBankCreditCardPeriodId,
                        model.SearchStartDate,
                        model.SearchEndDate,
                        model.SearchNextPeriodId,
                        model.SearchExcludeId,
                        model.SearchNotes,
                        SortField,
                        SortDescending,
                        model.SearchViaForm ? 0 : request.Page - 1,
                        request.PageSize));
        }

        [ActionName("_ExpenseEdit")]
        public PartialViewResult ExpenseEdit(Guid? id, int? typeId, Guid? storeId, Guid? groupId, Guid? creditCardId, Guid? creditCardPeriodId) {
            var model = new ExpenseEditWebModel {
                TypeId = (typeId ?? 0) != 0
                                    ? (typeId ?? 0)
                                    : (creditCardId.HasValue || creditCardPeriodId.HasValue
                                           ? (int)ExpenseTypeEnum.CreditCard
                                           : 0),
                ExpenseStoreId = storeId ?? default(Guid),
                ExpenseGroupId = groupId ?? default(Guid),
                ExpenseDateTime = DateTime.Today,
                BankCreditCardId = creditCardId ?? default(Guid),
                BankCreditCardPeriodId = creditCardPeriodId ?? default(Guid),
            };

            if ((id ?? Guid.Empty) != Guid.Empty) {
                var dataModel = expenseService.GetExpenseById(id ?? Guid.Empty, workContext.CurrentUser.Id);
                if (dataModel == null) {
                    return PartialView(model);
                }

                model = Engine.AutoMapperConfiguration.Mapper.Map<ExpenseDataModel, ExpenseEditWebModel>(dataModel);
            }

            model.TypeList = GetSelectList(commonService.GetTypeList((int)ExpenseTypeEnum.Parent));
            model.ExpenseStoreList = GetSelectList(expenseService.GetExpenseStoreList(workContext.CurrentUser.Id));
            model.ExpenseGroupList = GetSelectList(expenseService.GetExpenseGroupList(workContext.CurrentUser.Id));
            model.BankCreditCardList = GetSelectList(bankCreditCardService.GetBankCreditCardList(workContext.CurrentUser.Id, null, model.BankCreditCardId));
            model.BankCreditCardPeriodList = GetSelectList(bankCreditCardService.GetBankCreditCardPeriodList(workContext.CurrentUser.Id, model.BankCreditCardId ?? Guid.Empty));
            model.BankAccountList = GetSelectList(bankAccountService.GetBankAccountList(workContext.CurrentUser.Id));

            return PartialView(model);
        }

        [HttpPost, ValidateInput(false), ActionName("_ExpenseEdit")]
        public ActionResult ExpenseEdit(ExpenseEditWebModel model) {
            if (model.Id != Guid.Empty) {
                ModelState.Remove("TypeId");
            }

            if (model.ExpenseStoreNotExists) {
                model.ExpenseStoreId = Guid.Empty;
                ModelState.Remove("ExpenseStoreId");
            }
            else {
                model.ExpenseStoreName = string.Empty;
                ModelState.Remove("ExpenseStoreName");
            }

            if (model.ExpenseGroupNotExists) {
                model.ExpenseGroupId = Guid.Empty;
                ModelState.Remove("ExpenseGroupId");
            }
            else {
                model.ExpenseGroupName = string.Empty;
                ModelState.Remove("ExpenseGroupName");
            }

            if (ModelState.IsValid) {
                string errMessage;
                expenseService.AddUpdateExpense(
                    model.Id,
                    workContext.CurrentUser.Id,
                    model.TypeId,
                    model.ExpenseDateTime,
                    model.ExpenseStoreId,
                    model.ExpenseStoreName,
                    model.ExpenseGroupId,
                    model.ExpenseGroupName,
                    model.Amount,
                    model.InstallmentNo,
                    model.InstallmentTotal,
                    model.BankCreditCardId,
                    model.BankCreditCardPeriodId,
                    model.IsExclusion,
                    model.Notes,
                    model.BankAccountId,
                    out errMessage);

                return Json(new { Success = string.IsNullOrEmpty(errMessage), Result = errMessage });
            }

            return Json(new { Success = false, Result = Framework.WebViewPage.ValidationSummary(ModelState, string.Empty) });
        }

        public ActionResult ExpenseGroupList([DataSourceRequest]DataSourceRequest request, ExpenseGroupListWebModel model) {
            if (!Request.IsAjaxRequest()) {
                return View(GetExpenseGroupListWebModel());
            }

            SetSort(request);
            return Json(GetExpenseGroupListWebModel(model.SearchName, SortField, SortDescending, model.SearchViaForm ? 0 : request.Page - 1, request.PageSize));
        }

        [ActionName("_ExpenseGroupEdit")]
        public PartialViewResult ExpenseGroupEdit(Guid? id) {
            var model = new ExpenseGroupEditWebModel();

            if ((id ?? Guid.Empty) != Guid.Empty) {
                var dataModel = expenseService.GetExpenseGroupById(id ?? Guid.Empty, workContext.CurrentUser.Id);
                if (dataModel == null) {
                    return PartialView(model);
                }

                model = Engine.AutoMapperConfiguration.Mapper.Map<ExpenseGroupDataModel, ExpenseGroupEditWebModel>(dataModel);
            }

            return PartialView(model);
        }

        [HttpPost, ValidateInput(false), ActionName("_ExpenseGroupEdit")]
        public ActionResult ExpenseGroupEdit(ExpenseGroupEditWebModel model) {
            if (ModelState.IsValid) {
                string errMessage;
                expenseService.AddUpdateExpenseGroup(model.Id, workContext.CurrentUser.Id, model.Name, out errMessage);
                return Json(new { Success = string.IsNullOrEmpty(errMessage), Result = errMessage });
            }

            return Json(new { Success = false, Result = Framework.WebViewPage.ValidationSummary(ModelState, string.Empty) });
        }

        public ActionResult ExpenseGroupReport(int? year) {
            var yearList = expenseService.GetExpenseYearList(workContext.CurrentUser.Id);
            var data = expenseService.GetExpenseGroupReport(workContext.CurrentUser.Id, year ?? DateTime.Today.Year);
            var model = new ExpenseGroupReportWebModel {
                Year = year ?? DateTime.Today.Year,
                YearList = yearList,
                Data = Engine.AutoMapperConfiguration.Mapper.Map<List<ViewExpenseGroupReportDataModel>, List<ViewExpenseGroupReportWebModel>>(data)
            };

            return View(model);
        }

        public ActionResult ExpenseStoreList([DataSourceRequest]DataSourceRequest request, ExpenseStoreListWebModel model) {
            if (!Request.IsAjaxRequest()) {
                return View(GetExpenseStoreListWebModel());
            }

            SetSort(request);
            return Json(GetExpenseStoreListWebModel(model.SearchName, SortField, SortDescending, model.SearchViaForm ? 0 : request.Page - 1, request.PageSize));
        }

        [ActionName("_ExpenseStoreEdit")]
        public PartialViewResult ExpenseStoreEdit(Guid? id) {
            var model = new ExpenseStoreEditWebModel();

            if ((id ?? Guid.Empty) != Guid.Empty) {
                var dataModel = expenseService.GetExpenseStoreById(id ?? Guid.Empty, workContext.CurrentUser.Id);
                if (dataModel == null) {
                    return PartialView(model);
                }

                model = Engine.AutoMapperConfiguration.Mapper.Map<ExpenseStoreDataModel, ExpenseStoreEditWebModel>(dataModel);
            }

            return PartialView(model);
        }

        [HttpPost, ValidateInput(false), ActionName("_ExpenseStoreEdit")]
        public ActionResult ExpenseStoreEdit(ExpenseStoreEditWebModel model) {
            if (ModelState.IsValid) {
                string errMessage;
                expenseService.AddUpdateExpenseStore(model.Id, workContext.CurrentUser.Id, model.Name, out errMessage);
                return Json(new { Success = string.IsNullOrEmpty(errMessage), Result = errMessage });
            }

            return Json(new { Success = false, Result = Framework.WebViewPage.ValidationSummary(ModelState, string.Empty) });
        }

        private ExpenseListWebModel GetExpenseListWebModel(int typeId = 0, Guid? storeId = null, Guid? groupId = null, Guid? creditCardId = null, Guid? creditCardPeriodId = null, DateTime? startDate = null, DateTime? endDate = null, int? nextPeriodId = null, int? excludeId = null, string notes = "", string sort = "", bool sortDescending = false, int? pageIndex = 0, int? pageSize = 20) {
            int total;
            decimal expenseMe;
            decimal expenseExclusion;

            if (string.IsNullOrEmpty(sort) && creditCardPeriodId.HasValue) {
                sort = "ExpenseDateTime";
                sortDescending = false;
            }

            var data = expenseService.GetExpenseList(
                workContext.CurrentUser.Id,
                creditCardId,
                creditCardPeriodId,
                typeId,
                storeId,
                groupId,
                startDate,
                endDate,
                nextPeriodId,
                excludeId,
                notes,
                sort,
                sortDescending,
                pageIndex ?? 0,
                pageSize ?? 100,
                out total,
                out expenseMe,
                out expenseExclusion);

            var model = Engine.AutoMapperConfiguration.Mapper.Map<List<ExpenseDataModel>, List<ExpenseWebModel>>(data);
            if (model.Any()) {
                var firstItem = model.FirstOrDefault();
                if (firstItem != null) {
                    firstItem.ExpenseMe = expenseMe.ToString("N2");
                    firstItem.ExpenseExclusion = expenseExclusion.ToString("N2");
                    firstItem.ExpenseTotal = (expenseMe + expenseExclusion).ToString("N2");
                }
            }

            return new ExpenseListWebModel {
                SearchBankCreditCardId = creditCardId,
                SearchBankCreditCardList = GetSelectList(bankCreditCardService.GetBankCreditCardList(workContext.CurrentUser.Id, null, null)),
                SearchBankCreditCardPeriodId = creditCardPeriodId,
                SearchBankCreditCardPeriodList = GetSelectList(bankCreditCardService.GetBankCreditCardPeriodList(workContext.CurrentUser.Id, creditCardId ?? Guid.Empty)),
                SearchTypeId = typeId,
                SearchTypeList = GetSelectList(commonService.GetTypeList((int)ExpenseTypeEnum.Parent)),
                SearchExpenseStoreId = storeId,
                SearchExpenseStoreList = GetSelectList(expenseService.GetExpenseStoreList(workContext.CurrentUser.Id)),
                SearchExpenseGroupId = groupId,
                SearchExpenseGroupList = GetSelectList(expenseService.GetExpenseGroupList(workContext.CurrentUser.Id)),
                SearchStartDate = startDate,
                SearchEndDate = endDate,
                SearchNextPeriodId = nextPeriodId ?? 0,
                SearchNextPeriodList = GetYesNoList(),
                SearchExcludeId = excludeId ?? 0,
                SearchExcludeList = GetYesNoList(),
                SearchNotes = notes,
                Data = model,
                DeleteData = new DeleteWebModel { Permission = PermissionFormEnum.Expense, Form = FormEnum.Expense, GridName = "expenseGrid" },
                Total = total
            };
        }

        private ExpenseGroupListWebModel GetExpenseGroupListWebModel(string name = "", string sort = "", bool sortDescending = false, int? pageIndex = 0, int? pageSize = 20) {
            int total;
            var data = expenseService.GetExpenseGroupList(workContext.CurrentUser.Id, name, sort, sortDescending, pageIndex ?? 0, pageSize ?? 100, out total);
            var model = Engine.AutoMapperConfiguration.Mapper.Map<List<ExpenseGroupDataModel>, List<ExpenseGroupWebModel>>(data);

            return new ExpenseGroupListWebModel {
                SearchName = name,
                Data = model,
                DeleteData = new DeleteWebModel { Permission = PermissionFormEnum.Expense, Form = FormEnum.ExpenseGroup, GridName = "expenseGroupGrid" },
                Total = total
            };
        }

        private ExpenseStoreListWebModel GetExpenseStoreListWebModel(string name = "", string sort = "", bool sortDescending = false, int? pageIndex = 0, int? pageSize = 20) {
            int total;
            var data = expenseService.GetExpenseStoreList(workContext.CurrentUser.Id, name, sort, sortDescending, pageIndex ?? 0, pageSize ?? 100, out total);
            var model = Engine.AutoMapperConfiguration.Mapper.Map<List<ExpenseStoreDataModel>, List<ExpenseStoreWebModel>>(data);

            return new ExpenseStoreListWebModel {
                SearchName = name,
                Data = model,
                DeleteData = new DeleteWebModel { Permission = PermissionFormEnum.Expense, Form = FormEnum.ExpenseStore, GridName = "expenseStoreGrid" },
                Total = total
            };
        }
    }
}