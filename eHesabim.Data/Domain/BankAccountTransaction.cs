using System;

namespace eHesabim.Data.Domain {
    public class BankAccountTransaction : BaseEntity<Guid> {
        public int UserId { get; set; }

        public Guid BankAccountId { get; set; }

        public virtual BankAccount BankAccount { get; set; }

        public DateTime TrnDateTime { get; set; }

        public string Name { get; set; }

        public int TypeId { get; set; }

        public decimal Amount { get; set; }
        
        public Guid? RelatedId { get; set; }

        public virtual BankAccountTransaction Related { get; set; }

        public Guid? CustomerTransactionId { get; set; }

        public virtual CustomerTransaction CustomerTransaction { get; set; }

        public Guid? BankCreditCardPaymentId { get; set; }

        public virtual BankCreditCardPayment BankCreditCardPayment { get; set; }

        public Guid? BankCreditSubId { get; set; }

        public virtual BankCreditSub BankCreditSub { get; set; }

        public Guid? ExpenseId { get; set; }

        public virtual Expense Expense { get; set; }
    }
}
