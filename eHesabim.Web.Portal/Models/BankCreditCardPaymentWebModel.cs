using System;

namespace eHesabim.Web.Portal.Models {
    public class BankCreditCardPaymentWebModel : BaseWebModel<Guid> {
        public string BankCreditCardName { get; set; }

        public DateTime PaymentDateTime { get; set; }

        public decimal Amount { get; set; }

        public bool HasPoint { get; set; }

        public string BankAccountName { get; set; }
    }
}