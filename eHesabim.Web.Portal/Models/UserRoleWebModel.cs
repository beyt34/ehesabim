namespace eHesabim.Web.Portal.Models {
    public class UserRoleWebModel : BaseWebModel<int> {
        public int RoleId { get; set; }

        public string RoleName { get; set; }
    }
}