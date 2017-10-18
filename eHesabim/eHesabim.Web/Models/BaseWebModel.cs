using System;

namespace eHesabim.Web.Portal.Models {
    public class BaseWebModel<TId> {
        public TId Id { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public bool SearchViaForm { get; set; }

        public int Total { get; set; }
        
        public string SuccessMessage { get; set; }

        public string ErrorMessage { get; set; }
    }
}