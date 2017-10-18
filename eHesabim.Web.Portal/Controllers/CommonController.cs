using System;
using System.Web.Mvc;
using eHesabim.Framework;
using eHesabim.Services;

namespace eHesabim.Web.Portal.Controllers {
    public class CommonController : BaseController {
        private readonly IBankAccountService bankAccountService;
        private readonly IBankCreditService bankCreditService;
        private readonly IBankCreditCardService bankCreditCardService;
        private readonly ICommonService commonService;
        private readonly ICustomerService customerService;
        private readonly IExpenseService expenseService;
        private readonly IUserService userService;
        private readonly IWorkContext workContext;

        public CommonController(IBankAccountService bankAccountService, IBankCreditService bankCreditService, IBankCreditCardService bankCreditCardService, ICommonService commonService, ICustomerService customerService, IExpenseService expenseService, IUserService userService, IWorkContext workContext) {
            this.bankAccountService = bankAccountService;
            this.bankCreditService = bankCreditService;
            this.bankCreditCardService = bankCreditCardService;
            this.commonService = commonService;
            this.customerService = customerService;
            this.expenseService = expenseService;
            this.userService = userService;
            this.workContext = workContext;
        }

        [HttpPost]
        public ActionResult GetCountyList(int? cityId) {
            var data = GetSelectList(commonService.GetCountyList(cityId ?? 0));
            return Json(data);
        }

        [HttpPost]
        public ActionResult GetBankCreditCardPeriodList(Guid? bankCreditCardId) {
            var data = GetSelectList(bankCreditCardService.GetBankCreditCardPeriodList(workContext.CurrentUser.Id, bankCreditCardId ?? Guid.Empty));
            return Json(data);
        }

        [HttpPost]
        public ActionResult Delete(PermissionFormEnum permissionId, FormEnum formId, string id) {
            var errMessage = string.Empty;

            switch (formId) {
                case FormEnum.User:
                    userService.DeleteUser(Convert.ToInt32(id));
                    break;
                case FormEnum.Customer:
                    customerService.DeleteCustomer(new Guid(id), workContext.CurrentUser.Id, out errMessage);
                    break;
                case FormEnum.CustomerTransaction:
                    customerService.DeleteCustomerTransaction(new Guid(id), workContext.CurrentUser.Id, out errMessage);
                    break;
                case FormEnum.BankAccount:
                    bankAccountService.DeleteBankAccount(new Guid(id), workContext.CurrentUser.Id, out errMessage);
                    break;
                case FormEnum.BankAccountTransaction:
                    bankAccountService.DeleteBankAccountTransaction(new Guid(id), workContext.CurrentUser.Id, out errMessage);
                    break;
                case FormEnum.BankCredit:
                    bankCreditService.DeleteBankCredit(new Guid(id), workContext.CurrentUser.Id, out errMessage);
                    break;
                case FormEnum.BankCreditSub:
                    bankCreditService.DeleteBankCreditSub(new Guid(id), workContext.CurrentUser.Id, out errMessage);
                    break;
                case FormEnum.BankCreditCard:
                    bankCreditCardService.DeleteBankCreditCard(new Guid(id), workContext.CurrentUser.Id, out errMessage);
                    break;
                case FormEnum.BankCreditCardPeriod:
                    bankCreditCardService.DeleteBankCreditCardPeriod(new Guid(id), workContext.CurrentUser.Id, out errMessage);
                    break;
                case FormEnum.BankCreditCardPayment:
                    bankCreditCardService.DeleteBankCreditCardPayment(new Guid(id), workContext.CurrentUser.Id, out errMessage);
                    break;
                case FormEnum.Expense:
                    expenseService.DeleteExpense(new Guid(id), workContext.CurrentUser.Id, out errMessage);
                    break;
                case FormEnum.ExpenseGroup:
                    expenseService.DeleteExpenseGroup(new Guid(id), workContext.CurrentUser.Id, out errMessage);
                    break;
                case FormEnum.ExpenseStore:
                    expenseService.DeleteExpenseStore(new Guid(id), workContext.CurrentUser.Id, out errMessage);
                    break;
            }

            ////errMessage = Framework.Localization.Messages.AccessDenied;
            
            return Json(new { Success = string.IsNullOrEmpty(errMessage), Result = errMessage });
        }
    }
}