using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class BankAccountEditWebModel : BaseWebModel<Guid> {
        [ResourceRequired("RequiredAccountType")]
        [Range(1, 999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredAccountType")]
        public int TypeId { get; set; }
        
        public SelectList TypeList { get; set; }

        [ResourceRequired("RequiredAccountName")]
        public string Name { get; set; }

        public int? BankId { get; set; }

        public SelectList BankList { get; set; }

        public string Iban { get; set; }

        public decimal? Limit { get; set; }

        public bool IsActive { get; set; }
    }
}