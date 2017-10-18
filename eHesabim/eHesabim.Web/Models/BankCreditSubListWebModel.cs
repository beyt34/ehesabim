using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace eHesabim.Web.Portal.Models {
    public class BankCreditSubListWebModel : BaseWebModel<Guid> {
        public Guid? SearchBankCreditId { get; set; }

        public SelectList SearchBankCreditList { get; set; }
        
        public IEnumerable<BankCreditSubWebModel> Data { get; set; }

        public DeleteWebModel DeleteData { get; set; }
    }
}