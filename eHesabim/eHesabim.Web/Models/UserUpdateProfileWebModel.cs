using System.ComponentModel.DataAnnotations;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class UserUpdateProfileWebModel : BaseWebModel<int> {
        [ResourceRequired("Required")]
        public string Name { get; set; }

        [ResourceRequired("Required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        public bool ShowCustomer { get; set; }

        public bool ShowAccount { get; set; }

        public bool ShowCredit { get; set; }

        public bool ShowCard { get; set; }

        public bool ShowExpense { get; set; }

        public bool ShowCustomerExclusion { get; set; }
    }
}