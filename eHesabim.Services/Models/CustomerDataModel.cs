using System;

namespace eHesabim.Services.Models {
    public class CustomerDataModel : BaseDataModel<Guid> {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public int? CityId { get; set; }

        public int? CountyId { get; set; }

        public string BirthDate { get; set; }

        public string Notes { get; set; }
        
        public bool IsExclusion { get; set; }

        public decimal Debit { get; set; }

        public decimal Claim { get; set; }

        public decimal Net { get; set; }

        public decimal DueDebit { get; set; }
    }
}
