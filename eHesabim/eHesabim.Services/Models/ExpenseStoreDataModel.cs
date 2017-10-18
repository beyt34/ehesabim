using System;

namespace eHesabim.Services.Models {
    public class ExpenseStoreDataModel : BaseDataModel<Guid> {
        public int UserId { get; set; }

        public string Name { get; set; }
    }
}
