using System.ComponentModel.DataAnnotations;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class UserEditWebModel : BaseWebModel<int> {
        [ResourceRequired("RequiredUserName")]
        public string Name { get; set; }

        [ResourceRequired("RequiredEmail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
    }
}
