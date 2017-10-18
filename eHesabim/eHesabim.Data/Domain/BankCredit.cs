using System;

namespace eHesabim.Data.Domain {
    public class BankCredit : BaseEntity<Guid> {
        public int UserId { get; set; }

        public int BankId { get; set; }

        public virtual Bank Bank { get; set; }

        public DateTime CreditDateTime { get; set; }

        public decimal Capital { get; set; }

        public decimal Rate { get; set; }

        public int Installment { get; set; }

        public decimal MonthlyPayment { get; set; }

        public decimal Expense { get; set; }
    }
}
