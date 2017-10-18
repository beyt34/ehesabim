using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class CustomerTransactionEditWebModel : BaseWebModel<Guid> {
        [ResourceRequired("RequiredCustomer")]
        public Guid CustomerId { get; set; }

        public SelectList CustomerList { get; set; }

        [ResourceRequired("RequiredDate")]
        public DateTime TrnDateTime { get; set; }

        [ResourceRequired("RequiredTrnName")]
        public string Name { get; set; }

        [ResourceRequired("RequiredTrnType")]
        [Range(1, 999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredTrnType")]
        public int TypeId { get; set; }

        public SelectList CustomerTransactionTypeList { get; set; }

        [ResourceRequired("RequiredAmount")]
        [ResourceRegularExpression("AmountMustBeNumber", @"[+]?[0-9]*\,?[0-9]?[0-9]")]
        [Range(0.01, 999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "AmountMustBeNumber")]
        public decimal Amount { get; set; }

        public DateTime? DueDateTime { get; set; }

        [Range(1, 999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "AmountMustBeNumber")]
        public int? InstallmentNo { get; set; }

        [Range(1, 999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "AmountMustBeNumber")]
        public int? InstallmentTotal { get; set; }

        public Guid? BankAccountId { get; set; }

        public SelectList BankAccountList { get; set; }

        public string FileName { get; set; }

        public string FullPath { get; set; }
    }
}