using System;

namespace eHesabim.Data.Domain {
    public class ExpenseGroup : BaseEntity<Guid> {
        public int UserId { get; set; }

        public string Name { get; set; }
    }
}
