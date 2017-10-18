using System;
using System.Collections.Generic;

namespace eHesabim.Web.Portal.Models {
    public class DashboardWebModel : BaseWebModel<Guid> {
        public IEnumerable<DashboardCustomerWebModel> Customers { get; set; }

        public IEnumerable<DashboardBankAccountWebModel> BankAccounts { get; set; }

        public IEnumerable<BankCreditWebModel> BankCredits { get; set; }

        public IEnumerable<BankCreditCardWebModel> BankCreditCards { get; set; }
    }
}