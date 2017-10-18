using System.Collections.Generic;

namespace eHesabim.Web.Portal.Models {
    public class UserListWebModel : BaseWebModel<int> {
        public string SearchName { get; set; }

        public string SearchEmail { get; set; }

        public IEnumerable<UserWebModel> Data { get; set; }

        public DeleteWebModel DeleteData { get; set; }
    }
}