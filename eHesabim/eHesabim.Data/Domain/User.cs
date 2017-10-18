using System;

namespace eHesabim.Data.Domain {
    public class User : BaseEntity<int> {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }

        public string FacebookId { get; set; }

        public Guid? PasswordGuid { get; set; }

        public DateTime? PasswordExpire { get; set; }

        public bool? ShowCustomer { get; set; }

        public bool? ShowAccount { get; set; }

        public bool? ShowCredit { get; set; }

        public bool? ShowCard { get; set; }

        public bool? ShowExpense { get; set; }

        public bool? ShowCustomerExclusion { get; set; }

        /// <summary>Gets or sets the role.</summary>
        public int? Role { get; set; }
    }
}
