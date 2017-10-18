using eHesabim.Services;

namespace eHesabim.Web.Portal.Models {
    public class DeleteWebModel {
        public string Id { get; set; }

        public PermissionFormEnum Permission { get; set; }

        public FormEnum Form { get; set; }

        public string GridName { get; set; }
    }
}