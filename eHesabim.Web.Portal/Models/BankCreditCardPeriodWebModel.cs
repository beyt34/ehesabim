using System;

namespace eHesabim.Web.Portal.Models {
    public class BankCreditCardPeriodWebModel : BaseWebModel<Guid> {
        public string BankCreditCardName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime PaymentDate { get; set; }
    }
}