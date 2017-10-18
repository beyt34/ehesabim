using System.ComponentModel.DataAnnotations;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class UserPasswordRecoveryWebModel : BaseWebModel<int> {
        [ResourceRequired("Required")]
        [DataType(DataType.EmailAddress)]
        [ResourceRegularExpression("MustBeEmailFormat", @"^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$")]
        public string Email { get; set; }
    }
}