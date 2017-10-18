using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace eHesabim.Web.Portal.Models {
    public class BankCreditListWebModel : BaseWebModel<Guid> {
        public int SearchBankId { get; set; }

        public SelectList SearchBankList { get; set; }
        
        public IEnumerable<BankCreditWebModel> Data { get; set; }
        
        public DeleteWebModel DeleteData { get; set; }
    }
}