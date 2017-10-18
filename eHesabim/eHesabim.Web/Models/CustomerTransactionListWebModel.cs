using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace eHesabim.Web.Portal.Models {
    public class CustomerTransactionListWebModel : BaseWebModel<Guid> {
        public Guid? SearchCustomerId { get; set; }

        public SelectList SearchCustomerList { get; set; }

        public string SearchName { get; set; }

        public DateTime? SearchStartDate { get; set; }

        public DateTime? SearchEndDate { get; set; }

        public int SearchExcludeId { get; set; }

        public SelectList SearchExcludeList { get; set; }

        public IEnumerable<CustomerTransactionWebModel> Data { get; set; }

        public DeleteWebModel DeleteData { get; set; }
    }
}