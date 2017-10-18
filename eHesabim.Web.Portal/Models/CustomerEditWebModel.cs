using System;
using System.Web.Mvc;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Models {
    public class CustomerEditWebModel : BaseWebModel<Guid> {
        [ResourceRequired("Required")]
        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public int? CityId { get; set; }

        public SelectList CityList { get; set; }

        public int? CountyId { get; set; }

        public SelectList CountyList { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Notes { get; set; }

        public bool IsExclusion { get; set; }
    }
}