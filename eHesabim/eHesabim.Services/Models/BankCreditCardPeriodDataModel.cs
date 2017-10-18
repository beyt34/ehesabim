using System;

namespace eHesabim.Services.Models {
    public class BankCreditCardPeriodDataModel : BaseDataModel<Guid> {
        public int UserId { get; set; }

        public Guid BankCreditCardId { get; set; }

        public string BankCreditCardName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime PaymentDate { get; set; }
    }
}
