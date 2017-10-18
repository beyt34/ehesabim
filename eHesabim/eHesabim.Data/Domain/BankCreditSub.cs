using System;

namespace eHesabim.Data.Domain {
    public class BankCreditSub : BaseEntity<Guid> {
        public int UserId { get; set; }

        public Guid BankCreditId { get; set; }

        public virtual BankCredit BankCredit { get; set; }

        public int Installment { get; set; }

        public DateTime InstallmentDateTime { get; set; }

        public DateTime? PaymentDateTime { get; set; }

        public decimal InstallmentAmount { get; set; }

        public decimal CapitalAmount { get; set; }

        public decimal InterestAmount { get; set; }
    }
}
