using System;

namespace eHesabim.Services.Models {
    public class BankAccountTransactionDataModel : BaseDataModel<Guid> {
        public int UserId { get; set; }

        public Guid BankAccountId { get; set; }

        public string BankAccountName { get; set; }

        public DateTime TrnDateTime { get; set; }

        public string Name { get; set; }

        public int TypeId { get; set; }

        public decimal Amount { get; set; }

        public decimal Debit { get; set; }

        public decimal Claim { get; set; }

        public Guid? RelatedBankAccountId { get; set; }

        public string Explanation { get; set; }
    }
}
