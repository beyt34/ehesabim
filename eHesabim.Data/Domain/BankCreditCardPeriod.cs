using System;

namespace eHesabim.Data.Domain {
    public class BankCreditCardPeriod : BaseEntity<Guid> {
        public int UserId { get; set; }

        public Guid BankCreditCardId { get; set; }

        public virtual BankCreditCard BankCreditCard { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime PaymentDate { get; set; }
    }
}
