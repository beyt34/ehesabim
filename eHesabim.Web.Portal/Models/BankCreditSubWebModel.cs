using System;

namespace eHesabim.Web.Portal.Models {
    public class BankCreditSubWebModel : BaseWebModel<Guid> {
        public string BankName { get; set; }

        public string CreditTerm { get; set; }

        public string CreditInfo { get; set; }

        public int Installment { get; set; }

        public DateTime InstallmentDateTime { get; set; }

        public DateTime? PaymentDateTime { get; set; }

        public decimal InstallmentAmount { get; set; }

        public decimal CapitalAmount { get; set; }

        public decimal InterestAmount { get; set; }

        public decimal RemainCapital { get; set; }
    }
}