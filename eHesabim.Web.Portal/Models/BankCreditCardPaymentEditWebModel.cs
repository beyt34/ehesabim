using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class BankCreditCardPaymentEditWebModel : BaseWebModel<Guid> {
        [ResourceRequired("RequiredBankCreditCard")]
        public Guid BankCreditCardId { get; set; }

        public SelectList BankCreditCardList { get; set; }

        [ResourceRequired("RequiredDate")]
        public DateTime PaymentDateTime { get; set; }

        [ResourceRequired("RequiredAmount")]
        [ResourceRegularExpression("AmountMustBeNumber", @"[+]?[0-9]*\,?[0-9]?[0-9]")]
        [Range(0.01, 999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "AmountMustBeNumber")]
        public decimal Amount { get; set; }

        public bool HasPoint { get; set; }

        public Guid? BankAccountId { get; set; }

        public SelectList BankAccountList { get; set; }
    }
}