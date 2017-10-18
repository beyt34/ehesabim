using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class BankAccountTransactionEditWebModel : BaseWebModel<Guid> {
        [ResourceRequired("RequiredBankAccount")]
        public Guid BankAccountId { get; set; }

        public SelectList BankAccountList { get; set; }

        [ResourceRequired("RequiredDate")]
        public DateTime TrnDateTime { get; set; }

        [ResourceRequired("RequiredTrnName")]
        public string Name { get; set; }

        [ResourceRequired("RequiredTrnType")]
        [Range(1, 999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredTrnType")]
        public int TypeId { get; set; }

        public SelectList TypeList { get; set; }

        [ResourceRequired("RequiredAmount")]
        [ResourceRegularExpression("AmountMustBeNumber", @"[+]?[0-9]*\,?[0-9]?[0-9]")]
        [Range(0.01, 999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "AmountMustBeNumber")]
        public decimal Amount { get; set; }

        public Guid? RelatedBankAccountId { get; set; }

        public SelectList RelatedBankAccountList { get; set; }

        public string Explanation { get; set; }
    }
}