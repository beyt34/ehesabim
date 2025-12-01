using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class BankCreditSubEditWebModel : BaseWebModel<Guid> {
        [ResourceRequired("RequiredBankCredit")]
        public Guid BankCreditId { get; set; }

        public SelectList BankCreditList { get; set; }

        public int Installment { get; set; }

        [ResourceRequired("RequiredDate")]
        public DateTime InstallmentDateTime { get; set; }

        public DateTime? PaymentDateTime { get; set; }

        [ResourceRequired("RequiredInstallment")]
        [ResourceRegularExpression("RequiredInstallment", @"[+]?[0-9]*\,?[0-9]?[0-9]")]
        [Range(0.01, 999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredInstallment")]
        public decimal InstallmentAmount { get; set; }

        [ResourceRequired("RequiredCapital")]
        [ResourceRegularExpression("RequiredCapital", @"[+]?[0-9]*\,?[0-9]?[0-9]")]
        [Range(0.01, 999999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredCapital")]
        public decimal CapitalAmount { get; set; }

        [ResourceRequired("RequiredInterest")]
        [ResourceRegularExpression("RequiredInterest", @"[+]?[0-9]*\,?[0-9]?[0-9]")]
        public decimal InterestAmount { get; set; }

        public Guid? BankAccountId { get; set; }

        public SelectList BankAccountList { get; set; }

        public Guid? ExpenseGroupId { get; set; }

        public SelectList ExpenseGroupList { get; set; }
    }
}