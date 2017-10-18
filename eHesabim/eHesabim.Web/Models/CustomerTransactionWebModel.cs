using System;

namespace eHesabim.Web.Portal.Models {
    public class CustomerTransactionWebModel : BaseWebModel<Guid> {
        public string CustomerName { get; set; }

        public DateTime TrnDateTime { get; set; }

        public string Name { get; set; }

        public decimal Debit { get; set; }

        public decimal Claim { get; set; }

        public DateTime? DueDateTime { get; set; }

        public string Installment { get; set; }
        
        public string DebitTotal { get; set; }

        public string ClaimTotal { get; set; }

        public string NetTotal { get; set; }
    }
}