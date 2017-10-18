using System;

namespace eHesabim.Services.Models {
    public class BankAccountDataModel : BaseDataModel<Guid> {
        public int TypeId { get; set; }

        public string TypeName { get; set; }

        public string Name { get; set; }

        public int? BankId { get; set; }

        public string BankName { get; set; }

        public string Iban { get; set; }

        public decimal? Limit { get; set; }

        public bool IsActive { get; set; }

        public decimal DebitTotal { get; set; }

        public decimal ClaimTotal { get; set; }

        public decimal NetTotal { get; set; }
    }
}
