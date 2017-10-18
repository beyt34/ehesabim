using System;
using System.Collections.Generic;

namespace eHesabim.Web.Portal.Models {
    public class ExpenseGroupListWebModel : BaseWebModel<Guid> {
        public string SearchName { get; set; }

        public IEnumerable<ExpenseGroupWebModel> Data { get; set; }

        public DeleteWebModel DeleteData { get; set; }
    }
}