using System;
using System.Web.Mvc;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class BankCreditCardPeriodEditWebModel : BaseWebModel<Guid> {
        [ResourceRequired("RequiredBankCreditCard")]
        public Guid BankCreditCardId { get; set; }

        public SelectList BankCreditCardList { get; set; }

        [ResourceRequired("RequiredStartDate")]
        public DateTime StartDate { get; set; }

        [ResourceRequired("RequiredEndDate")]
        public DateTime EndDate { get; set; }

        [ResourceRequired("RequiredPaymentDate")]
        public DateTime PaymentDate { get; set; }

        public bool SetExpense { get; set; }
    }
}