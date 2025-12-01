using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class BankCreditCardEditWebModel : BaseWebModel<Guid> {
        public Guid? ParentId { get; set; }

        public SelectList ParentList { get; set; }

        [ResourceRequired("RequiredBank")]
        [Range(1, 999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredBank")]
        public int BankId { get; set; }

        public SelectList BankList { get; set; }

        [ResourceRequired("RequiredCardName")]
        public string Name { get; set; }

        public string CardNo { get; set; }

        [ResourceRegularExpression("AmountMustBeNumber", @"[+]?[0-9]*\,?[0-9]?[0-9]")]
        [Range(0.00, 999999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "AmountMustBeNumber")]
        public decimal Limit { get; set; }

        public string ExpiredMonth { get; set; }

        public string ExpiredYear { get; set; }

        public int? Order { get; set; }

        public bool IsActive { get; set; }
    }
}