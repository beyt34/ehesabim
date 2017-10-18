using System;

namespace eHesabim.Web.Portal.Models {
    public class DashboardBankAccountWebModel : BaseWebModel<Guid> {
        public string Name { get; set; }

        public decimal DebitTotal { get; set; }

        public decimal ClaimTotal { get; set; }

        public decimal NetTotal { get; set; }
    }
}