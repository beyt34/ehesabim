using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace eHesabim.Web.Portal.Models {
    public class CustomerListWebModel : BaseWebModel<Guid> {
        public string SearchName { get; set; }

        public string SearchEmail { get; set; }

        public int SearchExcludeId { get; set; }

        public SelectList SearchExcludeList { get; set; }

        public int SearchActiveId { get; set; }

        public SelectList SearchActiveList { get; set; }

        public IEnumerable<CustomerWebModel> Data { get; set; }

        public DeleteWebModel DeleteData { get; set; }
    }
}