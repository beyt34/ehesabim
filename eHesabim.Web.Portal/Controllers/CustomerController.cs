using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eHesabim.Core.Storage;
using eHesabim.Framework;
using eHesabim.Services;
using eHesabim.Services.Models;
using eHesabim.Web.Portal.Export;
using eHesabim.Web.Portal.Models;
using Kendo.Mvc.UI;

namespace eHesabim.Web.Portal.Controllers {
    /// <summary>The customer controller.</summary>
    public class CustomerController : BaseController {
        /// <summary>The bank account service.</summary>
        private readonly IBankAccountService bankAccountService;

        /// <summary>The common service.</summary>
        private readonly ICommonService commonService;

        /// <summary>The customer service.</summary>
        private readonly ICustomerService customerService;

        /// <summary>The storage service.</summary>
        private readonly IStorageService storageService;

        /// <summary>The work context.</summary>
        private readonly IWorkContext workContext;

        public CustomerController(IBankAccountService bankAccountService, ICommonService commonService, ICustomerService customerService, IStorageService storageService, IWorkContext workContext) {
            this.bankAccountService = bankAccountService;
            this.commonService = commonService;
            this.customerService = customerService;
            this.storageService = storageService;
            this.workContext = workContext;
        }

        public ActionResult CustomerList([DataSourceRequest]DataSourceRequest request, CustomerListWebModel model) {
            if (!Request.IsAjaxRequest()) {
                return View(GetCustomerListWebModel());
            }

            SetSort(request);
            return
                Json(
                    GetCustomerListWebModel(
                        model.SearchName,
                        model.SearchEmail,
                        SortField,
                        SortDescending,
                        model.SearchViaForm ? 0 : request.Page - 1,
                        request.PageSize));
        }

        [ActionName("_CustomerEdit")]
        public PartialViewResult CustomerEdit(Guid? id) {
            var model = new CustomerEditWebModel();

            if ((id ?? Guid.Empty) != Guid.Empty) {
                var dataModel = customerService.GetCustomerById(id ?? Guid.Empty, workContext.CurrentUser.Id);
                if (dataModel == null) {
                    return PartialView(model);
                }

                model = Engine.AutoMapperConfiguration.Mapper.Map<CustomerDataModel, CustomerEditWebModel>(dataModel);
            }

            model.CityList = GetSelectList(commonService.GetCityList());
            model.CountyList = GetSelectList(commonService.GetCountyList(model.CityId ?? 0));

            return PartialView(model);
        }

        [HttpPost, ValidateInput(false), ActionName("_CustomerEdit")]
        public ActionResult CustomerEdit(CustomerEditWebModel model) {
            if (ModelState.IsValid) {
                string errMessage;
                customerService.AddUpdateCustomer(
                    model.Id,
                    workContext.CurrentUser.Id,
                    model.Name,
                    model.Email,
                    model.Phone,
                    model.Address,
                    model.CityId,
                    model.CountyId,
                    model.BirthDate,
                    model.Notes,
                    model.IsExclusion,
                    out errMessage);

                return Json(new { Success = string.IsNullOrEmpty(errMessage), Result = errMessage });
            }

            return Json(new { Success = false, Result = Framework.WebViewPage.ValidationSummary(ModelState, string.Empty) });
        }

        public ActionResult CustomerTransactionList([DataSourceRequest]DataSourceRequest request, CustomerTransactionListWebModel model) {
            if (!Request.IsAjaxRequest()) {
                return View(GetCustomerTransactionListWebModel());
            }

            SetSort(request);
            return
                Json(
                    GetCustomerTransactionListWebModel(
                        model.SearchCustomerId,
                        model.SearchName,
                        model.SearchStartDate,
                        model.SearchEndDate,
                        model.SearchExcludeId,
                        SortField,
                        SortDescending,
                        model.SearchViaForm ? 0 : request.Page - 1,
                        request.PageSize));
        }

        [ActionName("_CustomerTransactionEdit")]
        public PartialViewResult CustomerTransactionEdit(Guid? id, Guid? customerId) {
            var model = new CustomerTransactionEditWebModel {
                CustomerId = customerId ?? default(Guid),
                TrnDateTime = DateTime.Today
            };

            if ((id ?? Guid.Empty) != Guid.Empty) {
                var dataModel = customerService.GetCustomerTransactionById(id ?? Guid.Empty, workContext.CurrentUser.Id);
                if (dataModel == null) {
                    return PartialView(model);
                }

                model = Engine.AutoMapperConfiguration.Mapper.Map<CustomerTransactionDataModel, CustomerTransactionEditWebModel>(dataModel);
            }

            model.CustomerList = GetSelectList(customerService.GetCustomerList(workContext.CurrentUser.Id));
            model.CustomerTransactionTypeList = GetSelectList(commonService.GetTypeList((int)CustomerTransactionTypeEnum.Parent));
            model.BankAccountList = GetSelectList(bankAccountService.GetBankAccountList(workContext.CurrentUser.Id));

            return PartialView(model);
        }

        [HttpPost, ValidateInput(false), ActionName("_CustomerTransactionEdit")]
        public ActionResult CustomerTransactionEdit(CustomerTransactionEditWebModel model) {
            if (ModelState.IsValid) {
                string errMessage;
                customerService.AddUpdateCustomerTransaction(
                    model.Id,
                    workContext.CurrentUser.Id,
                    model.CustomerId,
                    model.TrnDateTime,
                    model.Name,
                    model.TypeId,
                    model.Amount,
                    model.DueDateTime,
                    model.InstallmentNo,
                    model.InstallmentTotal,
                    model.BankAccountId,
                    model.FileName,
                    out errMessage);

                return Json(new { Success = string.IsNullOrEmpty(errMessage), Result = errMessage });
            }

            return Json(new { Success = false, Result = Framework.WebViewPage.ValidationSummary(ModelState, string.Empty) });
        }

        public ActionResult CustomerAbstract() {
            var model = new CustomerAbstractWebModel {
                CustomerList = GetSelectList(customerService.GetCustomerList(workContext.CurrentUser.Id)),
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult CustomerAbstract(CustomerAbstractWebModel model) {
            if (ModelState.IsValid) {
                string customerName;
                var report = customerService.GetCustomerAbstract(workContext.CurrentUser.Id, model.CustomerId ?? Guid.Empty, model.StartDate, model.EndDate, out customerName);

                var columns = ExcelColumns.GetCustomerAbstractColumns();
                var result = ExportToExcel.ExportExcel(report, columns, customerName);
                return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("{0}.xlsx", customerName));
            }

            return View(model);
        }
        
        public ActionResult UploadFile(Guid id, HttpPostedFileBase file) {
            if (file == null) {
                return Content(string.Empty);
            }

            // upload file
            var fileName = string.Format("{0}{1}", Guid.NewGuid(), Path.GetExtension(file.FileName));
            storageService.Upload(file.InputStream, "files", fileName, file.ContentType);

            // update db
            customerService.UpdateCustomerTransactionFileName(id, fileName);

            // return
            var fullPath = string.Format("{0}{1}/{2}", ConfigurationManager.AppSettings["FilePath"], "files", fileName);
            return Json(new { Result = true, FileName = fileName, FullPath = fullPath }, "text/plain");
        }

        [HttpPost]
        public ActionResult DeleteFile(Guid id, string fileName) {
            // delete file
            storageService.Delete("files", fileName);

            // update db
            customerService.UpdateCustomerTransactionFileName(id, string.Empty);

            // return
            return Json(new { Result = true }, "text/plain");
        }

        private CustomerListWebModel GetCustomerListWebModel(string name = "", string email = "", string sort = "", bool sortDescending = false, int? pageIndex = 0, int? pageSize = 20) {
            int total;
            decimal debitTotal;
            decimal claimTotal;

            var data = customerService.GetCustomerList(
                workContext.CurrentUser.Id,
                name,
                email,
                sort,
                sortDescending,
                pageIndex ?? 0,
                pageSize ?? 100,
                out total,
                out debitTotal,
                out claimTotal);

            var model = Engine.AutoMapperConfiguration.Mapper.Map<List<CustomerDataModel>, List<CustomerWebModel>>(data);
            if (model.Any()) {
                var firstItem = model.FirstOrDefault();
                if (firstItem != null) {
                    firstItem.DebitTotal = debitTotal.ToString("N2");
                    firstItem.ClaimTotal = claimTotal.ToString("N2");
                    firstItem.NetTotal = (debitTotal - claimTotal).ToString("N2");
                }
            }

            return new CustomerListWebModel {
                SearchName = name,
                SearchEmail = email,
                Data = model,
                DeleteData = new DeleteWebModel { Permission = PermissionFormEnum.Customer, Form = FormEnum.Customer, GridName = "customerGrid" },
                Total = total
            };
        }

        private CustomerTransactionListWebModel GetCustomerTransactionListWebModel(Guid? customerId = null, string name = "", DateTime? startDate = null, DateTime? endDate = null, int? excludeId = null, string sort = "", bool sortDescending = false, int? pageIndex = 0, int? pageSize = 20) {
            int total;
            decimal debtTotal;
            decimal claimTotal;

            var data = customerService.GetCustomerTransactionList(
                workContext.CurrentUser.Id,
                customerId,
                name,
                startDate,
                endDate,
                excludeId,
                sort,
                sortDescending,
                pageIndex ?? 0,
                pageSize ?? 100,
                out total,
                out debtTotal,
                out claimTotal);

            var model = Engine.AutoMapperConfiguration.Mapper.Map<List<CustomerTransactionDataModel>, List<CustomerTransactionWebModel>>(data);
            if (model.Any()) {
                var firstItem = model.FirstOrDefault();
                if (firstItem != null) {
                    firstItem.DebitTotal = debtTotal.ToString("N2");
                    firstItem.ClaimTotal = claimTotal.ToString("N2");
                    firstItem.NetTotal = (debtTotal - claimTotal).ToString("N2");
                }
            }

            return new CustomerTransactionListWebModel {
                SearchCustomerId = customerId,
                SearchCustomerList = GetSelectList(customerService.GetCustomerList(workContext.CurrentUser.Id)),
                SearchStartDate = startDate,
                SearchEndDate = endDate,
                SearchExcludeId = excludeId ?? 0,
                SearchExcludeList = GetYesNoList(),
                SearchName = name,
                Data = model,
                DeleteData = new DeleteWebModel { Permission = PermissionFormEnum.Customer, Form = FormEnum.CustomerTransaction, GridName = "customerTransactionGrid" },
                Total = total
            };
        }
    }
}
