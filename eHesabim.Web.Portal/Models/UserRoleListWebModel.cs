using System.Collections.Generic;
using System.Web.Mvc;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class UserRoleListWebModel : BaseWebModel<int> {
        [ResourceRequired("Required")]
        public int RoleId { get; set; }

        public IEnumerable<SelectListItem> RoleList { get; set; }

        public IEnumerable<UserRoleWebModel> Data { get; set; }
    }
}