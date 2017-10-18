using System;

namespace eHesabim.Web.Portal.Models {
    public class BankCreditCardWebModel : BaseWebModel<Guid> {
        public string Name { get; set; }

        public decimal Limit { get; set; }

        public int? Order { get; set; }

        public bool IsActive { get; set; }

        public decimal AvailableLimit { get; set; }

        public DateTime EndDateTime { get; set; }

        public DateTime PaymentDateTime { get; set; }

        public decimal LastDebt { get; set; }

        public decimal CurrentDebt { get; set; }

        public decimal NextDebt { get; set; }

        public decimal DebtTotal { get; set; }
    }
}