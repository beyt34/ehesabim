using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace eHesabim.Web.Portal.Models {
    public class BankAccountTransactionListWebModel : BaseWebModel<Guid> {
        public Guid? SearchBankAccountId { get; set; }

        public SelectList SearchBankAccountList { get; set; }

        public string SearchName { get; set; }

        public DateTime? SearchStartDate { get; set; }

        public DateTime? SearchEndDate { get; set; }

        public IEnumerable<BankAccountTransactionWebModel> Data { get; set; }

        public DeleteWebModel DeleteData { get; set; }
    }
}