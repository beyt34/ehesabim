using System;

namespace eHesabim.Services.Models {
    public class ExpenseGroupDataModel : BaseDataModel<Guid> {
        public int UserId { get; set; }

        public string Name { get; set; }
    }
}
