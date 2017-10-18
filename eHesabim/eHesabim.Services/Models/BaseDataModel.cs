using System;

namespace eHesabim.Services.Models {
    public class BaseDataModel<TId> {
        public TId Id { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime? UpdatedDateTime { get; set; }
    }
}
