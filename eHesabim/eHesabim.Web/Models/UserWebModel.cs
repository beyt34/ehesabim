namespace eHesabim.Web.Portal.Models {
    public class UserWebModel : BaseWebModel<int> {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}