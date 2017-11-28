using System;

namespace eHesabim.Web.Portal.Models {
    public class CustomerWebModel : BaseWebModel<Guid> {
        public string Name { get; set; }

        public string Email { get; set; }

        public decimal Debit { get; set; }

        public decimal Claim { get; set; }

        public decimal Net { get; set; }

        public bool IsExclusion { get; set; }

        public bool IsActive { get; set; }

        public string DebitTotal { get; set; }

        public string ClaimTotal { get; set; }

        public string NetTotal { get; set; }
    }
}