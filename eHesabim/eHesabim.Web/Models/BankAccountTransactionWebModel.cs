using System;

namespace eHesabim.Web.Portal.Models {
    public class BankAccountTransactionWebModel : BaseWebModel<Guid> {
        public string BankAccountName { get; set; }

        public DateTime TrnDateTime { get; set; }

        public string Name { get; set; }

        public decimal Debit { get; set; }

        public decimal Claim { get; set; }
        
        /* dummy fields for sub total */
        public string DebitTotal { get; set; }

        public string ClaimTotal { get; set; }

        public string NetTotal { get; set; }
    }
}