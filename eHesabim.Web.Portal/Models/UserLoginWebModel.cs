using System.ComponentModel.DataAnnotations;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class UserLoginWebModel : BaseWebModel<int> {
        [ResourceRequired("RequiredEmail")]
        [DataType(DataType.EmailAddress)]
        [ResourceRegularExpression("MustBeEmailFormat", @"^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$")]
        public string Email { get; set; }

        [ResourceRequired("RequiredPassword")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}