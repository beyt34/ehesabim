using System;

namespace eHesabim.Data.Domain {
    public class CustomerTransaction : BaseEntity<Guid> {
        public int UserId { get; set; }

        public Guid? ParentId { get; set; }

        public virtual CustomerTransaction Parent { get; set; }

        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        public DateTime TrnDateTime { get; set; }

        public string Name { get; set; }

        public int TypeId { get; set; }

        public decimal Amount { get; set; }

        public DateTime? DueDateTime { get; set; }

        public int? InstallmentNo { get; set; }

        public int? InstallmentTotal { get; set; }

        public string FileName { get; set; }
    }
}
