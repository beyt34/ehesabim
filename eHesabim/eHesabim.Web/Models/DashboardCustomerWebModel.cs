using System;

namespace eHesabim.Web.Portal.Models {
    public class DashboardCustomerWebModel : BaseWebModel<Guid> {
        public string Name { get; set; }

        public bool IsExclusion { get; set; }

        public decimal Debit { get; set; }

        public decimal Claim { get; set; }

        public decimal Net { get; set; }

        public decimal DueDebit { get; set; }
    }
}