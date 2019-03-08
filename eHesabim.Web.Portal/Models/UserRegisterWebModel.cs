using System.ComponentModel.DataAnnotations;

using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class UserRegisterWebModel : BaseWebModel<int> {
        [ResourceRequired("RequiredUserName")]
        public string Name { get; set; }

        [ResourceRequired("RequiredEmail")]
        [DataType(DataType.EmailAddress)]
        [ResourceRegularExpression("MustBeEmailFormat", @"^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$")]
        public string Email { get; set; }

        [ResourceRequired("RequiredPassword")]
        [ResourceStringLength("StringLengthPassword", 10, 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }
    }
}