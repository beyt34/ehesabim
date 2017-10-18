using System;

namespace eHesabim.Web.Portal.Models {
    public class BankCreditWebModel : BaseWebModel<Guid> {
        public string BankName { get; set; }

        public DateTime CreditDateTime { get; set; }

        public decimal Capital { get; set; }

        public int Installment { get; set; }

        public decimal MonthlyPayment { get; set; }

        public decimal RemainCapital { get; set; }

        public int RemainInstallment { get; set; }

        public DateTime PaymentDateTime { get; set; }
    }
}