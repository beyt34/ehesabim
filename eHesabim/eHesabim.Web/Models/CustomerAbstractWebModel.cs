using System;
using System.Web.Mvc;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class CustomerAbstractWebModel : BaseWebModel<Guid> {
        [ResourceRequired("RequiredCustomer")]
        public Guid? CustomerId { get; set; }

        public SelectList CustomerList { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}