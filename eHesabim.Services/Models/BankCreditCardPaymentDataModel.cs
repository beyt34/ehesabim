using System;

namespace eHesabim.Services.Models {
    public class BankCreditCardPaymentDataModel : BaseDataModel<Guid> {
        public int UserId { get; set; }

        public Guid BankCreditCardId { get; set; }

        public string BankCreditCardName { get; set; }

        public DateTime PaymentDateTime { get; set; }

        public decimal Amount { get; set; }

        public bool HasPoint { get; set; }

        public Guid? BankAccountId { get; set; }
        
        public string BankAccountName { get; set; }
    }
}
