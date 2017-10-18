using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class BankCreditEditWebModel : BaseWebModel<Guid> {
        [ResourceRequired("RequiredBank")]
        [Range(1, 999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredBank")]
        public int BankId { get; set; }

        public SelectList BankList { get; set; }

        [ResourceRequired("RequiredDate")]
        public DateTime CreditDateTime { get; set; }

        [ResourceRequired("RequiredCapital")]
        [ResourceRegularExpression("AmountMustBeNumber", @"[+]?[0-9]*\,?[0-9]?[0-9]")]
        [Range(0.01, 999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "AmountMustBeNumber")]
        public decimal Capital { get; set; }

        [ResourceRequired("RequiredRate")]
        [ResourceRegularExpression("AmountMustBeNumber", @"[+]?[0-9]*\,?[0-9]?[0-9]")]
        [Range(0.01, 100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "ValueMustBeZeroHundred")]
        public decimal Rate { get; set; }

        [ResourceRequired("RequiredInstallment")]
        [ResourceRegularExpression("AmountMustBeNumber", @"[+]?[0-9]*\,?[0-9]?[0-9]")]
        [Range(1, 120, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "ValueMustBeOneHundredTwenty")]
        public int Installment { get; set; }

        [ResourceRequired("RequiredMonthlyPayment")]
        [ResourceRegularExpression("AmountMustBeNumber", @"[+]?[0-9]*\,?[0-9]?[0-9]")]
        [Range(0.00, 999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "AmountMustBeNumber")]
        public decimal MonthlyPayment { get; set; }

        [ResourceRequired("RequiredExpense")]
        [ResourceRegularExpression("AmountMustBeNumber", @"[+]?[0-9]*\,?[0-9]?[0-9]")]
        [Range(0.00, 999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "AmountMustBeNumber")]
        public decimal Expense { get; set; }
    }
}