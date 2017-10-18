using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace eHesabim.Web.Portal.Models {
    public class BankAccountListWebModel : BaseWebModel<Guid> {
        public int SearchTypeId { get; set; }

        public SelectList SearchTypeList { get; set; }

        public int SearchBankId { get; set; }

        public SelectList SearchBankList { get; set; }

        public IEnumerable<BankAccountWebModel> Data { get; set; }

        public DeleteWebModel DeleteData { get; set; }
    }
}