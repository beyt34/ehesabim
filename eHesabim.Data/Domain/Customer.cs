using System;

namespace eHesabim.Data.Domain {
    public class Customer : BaseEntity<Guid> {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public int? CityId { get; set; }

        public int? CountyId { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Notes { get; set; }

        public bool IsExclusion { get; set; }

        public bool IsActive { get; set; }
    }
}
