using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class ExpenseEditWebModel : BaseWebModel<Guid> {
        [ResourceRequired("RequiredExpenseType")]
        [Range(1, 999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredExpenseType")]
        public int TypeId { get; set; }

        public SelectList TypeList { get; set; }

        [ResourceRequired("RequiredDate")]
        public DateTime ExpenseDateTime { get; set; }

        [ResourceRequired("RequiredExpenseStore")]
        public Guid ExpenseStoreId { get; set; }

        public SelectList ExpenseStoreList { get; set; }

        public bool ExpenseStoreNotExists { get; set; }

        [ResourceRequired("RequiredExpenseStoreName")]
        public string ExpenseStoreName { get; set; }

        [ResourceRequired("RequiredExpenseGroup")]
        public Guid ExpenseGroupId { get; set; }

        public SelectList ExpenseGroupList { get; set; }

        public bool ExpenseGroupNotExists { get; set; }

        [ResourceRequired("RequiredExpenseGroupName")]
        public string ExpenseGroupName { get; set; }

        [ResourceRequired("RequiredAmount")]
        [ResourceRegularExpression("AmountMustBeNumber", @"[-]?[+]?[0-9]*\,?[0-9]?[0-9]")]
        [Range(-999999, 999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "AmountMustBeNumber")]
        public decimal Amount { get; set; }

        [Range(1, 999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "AmountMustBeNumber")]
        public int? InstallmentNo { get; set; }

        [Range(1, 999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "AmountMustBeNumber")]
        public int? InstallmentTotal { get; set; }

        public Guid? BankCreditCardId { get; set; }

        public SelectList BankCreditCardList { get; set; }

        public Guid? BankCreditCardPeriodId { get; set; }

        public SelectList BankCreditCardPeriodList { get; set; }

        public bool IsExclusion { get; set; }

        public string Notes { get; set; }

        public Guid? BankAccountId { get; set; }

        public SelectList BankAccountList { get; set; }

        public string Explanation { get; set; }
    }
}