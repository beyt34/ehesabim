using System;
using System.ComponentModel.DataAnnotations;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class UserPasswordRecoveryConfirmWebModel : BaseWebModel<int> {
        public Guid PasswordGuid { get; set; }

        [ResourceRequired("RequiredPassword")]
        [ResourceStringLength("StringLengthPassword", 10, 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [ResourceRequired("RequiredRePassword")]
        [ResourceDataAnnotationsCompare("Password", "PasswordMismatch")]
        [DataType(DataType.Password)]
        public string RePassword { get; set; }
    }
}