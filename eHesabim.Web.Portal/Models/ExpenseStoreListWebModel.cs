using System;
using System.Collections.Generic;

namespace eHesabim.Web.Portal.Models {
    public class ExpenseStoreListWebModel : BaseWebModel<Guid> {
        public string SearchName { get; set; }

        public IEnumerable<ExpenseStoreWebModel> Data { get; set; }

        public DeleteWebModel DeleteData { get; set; }
    }
}