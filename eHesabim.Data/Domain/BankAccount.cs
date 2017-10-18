using System;

namespace eHesabim.Data.Domain {
    public class BankAccount : BaseEntity<Guid> {
        public int UserId { get; set; }

        public int TypeId { get; set; }

        public virtual Type Type { get; set; }

        public string Name { get; set; }

        public int? BankId { get; set; }

        public virtual Bank Bank { get; set; }

        public string Iban { get; set; }

        public decimal? Limit { get; set; }

        public bool IsActive { get; set; }
    }
}
