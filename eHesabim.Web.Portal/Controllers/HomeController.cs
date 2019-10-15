using System.Collections.Generic;
using System.Web.Mvc;
using eHesabim.Framework;
using eHesabim.Services;
using eHesabim.Services.Models;
using eHesabim.Web.Portal.Models;

namespace eHesabim.Web.Portal.Controllers {
    public class HomeController : BaseController {
        private readonly ICustomerService customerService;
        private readonly IBankAccountService bankAccountService;
        private readonly IBankCreditService bankCreditService;
        private readonly IBankCreditCardService bankCreditCardService;
        private readonly IWorkContext workContext;

        public HomeController(ICustomerService customerService, IBankAccountService bankAccountService, IBankCreditService bankCreditService, IBankCreditCardService bankCreditCardService, IWorkContext workContext) {
            this.customerService = customerService;
            this.bankAccountService = bankAccountService;
            this.bankCreditService = bankCreditService;
            this.bankCreditCardService = bankCreditCardService;
            this.workContext = workContext;
        }

        public ActionResult Index() {
            var model = new DashboardWebModel();

            //// show customer.
            if (workContext.CurrentUser.ShowCustomer) {
                var customers = customerService.GetDashboardCustomerList(workContext.CurrentUser.Id, workContext.CurrentUser.ShowCustomerExclusion);
                model.Customers = Engine.AutoMapperConfiguration.Mapper.Map<List<CustomerDataModel>, List<DashboardCustomerWebModel>>(customers);
            }

            //// show account
            if (workContext.CurrentUser.ShowAccount) {
                var bankAccounts = bankAccountService.GetDashboardBankAccountList(workContext.CurrentUser.Id);
                model.BankAccounts = Engine.AutoMapperConfiguration.Mapper.Map<List<BankAccountDataModel>, List<DashboardBankAccountWebModel>>(bankAccounts);
            }

            //// show credit
            if (workContext.CurrentUser.ShowCredit) {
                var bankCredits = bankCreditService.GetDashboardBankCreditList(workContext.CurrentUser.Id);
                model.BankCredits = Engine.AutoMapperConfiguration.Mapper.Map<List<BankCreditDataModel>, List<BankCreditWebModel>>(bankCredits);
            }

            //// show card
            if (workContext.CurrentUser.ShowCard) {
                var bankCreditCards = bankCreditCardService.GetDashboardBankCreditCardList(workContext.CurrentUser.Id);
                model.BankCreditCards = Engine.AutoMapperConfiguration.Mapper.Map<List<BankCreditCardDataModel>, List<BankCreditCardWebModel>>(bankCreditCards);
            }

            return View(model);
        }
    }
}
