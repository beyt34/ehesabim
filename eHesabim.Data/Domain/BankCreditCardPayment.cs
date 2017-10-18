using System;
using System.Collections.Generic;

namespace eHesabim.Data.Domain {
    public class BankCreditCardPayment : BaseEntity<Guid> {
        public int UserId { get; set; }

        public Guid BankCreditCardId { get; set; }

        public virtual BankCreditCard BankCreditCard { get; set; }

        public DateTime PaymentDateTime { get; set; }

        public decimal Amount { get; set; }

        public bool HasPoint { get; set; }

        public virtual ICollection<BankAccountTransaction> BankAccountTransactions { get; set; }
    }
}
