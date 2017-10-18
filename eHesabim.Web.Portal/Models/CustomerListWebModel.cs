using System;
using System.Collections.Generic;

namespace eHesabim.Web.Portal.Models {
    public class CustomerListWebModel : BaseWebModel<Guid> {
        public string SearchName { get; set; }

        public string SearchEmail { get; set; }

        public IEnumerable<CustomerWebModel> Data { get; set; }

        public DeleteWebModel DeleteData { get; set; }
    }
}