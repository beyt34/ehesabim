using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace eHesabim.Web.Portal.Models {
    public class BankCreditCardPaymentListWebModel : BaseWebModel<Guid> {
        public Guid? SearchBankCreditCardId { get; set; }

        public SelectList SearchBankCreditCardList { get; set; }

        public IEnumerable<BankCreditCardPaymentWebModel> Data { get; set; }

        public DeleteWebModel DeleteData { get; set; }
    }
}