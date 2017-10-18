using System;

namespace eHesabim.Web.Portal.Models {
    public class BankAccountWebModel : BaseWebModel<Guid> {
        public string TypeName { get; set; }

        public string Name { get; set; }

        public string BankName { get; set; }

        public string Iban { get; set; }

        public decimal? Limit { get; set; }

        public bool IsActive { get; set; }
    }
}