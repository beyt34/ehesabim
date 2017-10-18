using System.ComponentModel.DataAnnotations;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class UserLoginWebModel : BaseWebModel<int> {
        [ResourceRequired("RequiredEmail")]
        [DataType(DataType.EmailAddress)]
        [ResourceRegularExpression("MustBeEmailFormat", @"^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$")]
        public string LoginEmail { get; set; }

        [ResourceRequired("RequiredPassword")]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }

        public bool LoginRememberMe { get; set; }

        public string LoginReturnUrl { get; set; }

        public string LoginErrorMessage { get; set; }

        [ResourceRequired("RequiredUserName")]
        public string RegisterName { get; set; }

        [ResourceRequired("RequiredEmail")]
        [DataType(DataType.EmailAddress)]
        [ResourceRegularExpression("MustBeEmailFormat", @"^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$")]
        public string RegisterEmail { get; set; }

        [ResourceRequired("RequiredPassword")]
        [ResourceStringLength("StringLengthPassword", 10, 6)]
        [DataType(DataType.Password)]
        public string RegisterPassword { get; set; }

        public string RegisterErrorMessage { get; set; }
    }
}