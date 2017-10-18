using System;

namespace eHesabim.Web.Portal.Models {
    public class ExpenseGroupWebModel : BaseWebModel<Guid> {
        public string Name { get; set; }
    }
}