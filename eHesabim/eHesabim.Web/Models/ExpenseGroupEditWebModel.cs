using System;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class ExpenseGroupEditWebModel : BaseWebModel<Guid> {
        [ResourceRequired("Required")]
        public string Name { get; set; }
    }
}