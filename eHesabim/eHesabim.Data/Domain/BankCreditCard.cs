using System;

namespace eHesabim.Data.Domain {
    public class BankCreditCard : BaseEntity<Guid> {
        public int UserId { get; set; }

        public Guid? ParentId { get; set; }

        public int BankId { get; set; }

        public virtual Bank Bank { get; set; }

        public string Name { get; set; }

        public string CardNo { get; set; }

        public decimal Limit { get; set; }

        public string ExpiredMonth { get; set; }

        public string ExpiredYear { get; set; }

        public int? Order { get; set; }

        public bool IsActive { get; set; }
    }
}
