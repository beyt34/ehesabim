using System;

namespace eHesabim.Services.Models {
    public class BankCreditCardDataModel : BaseDataModel<Guid> {
        public int UserId { get; set; }

        public Guid? ParentId { get; set; }

        public int BankId { get; set; }

        public string BankName { get; set; }

        public string Name { get; set; }

        public string CardNo { get; set; }

        public decimal Limit { get; set; }

        public string ExpiredMonth { get; set; }

        public string ExpiredYear { get; set; }

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
