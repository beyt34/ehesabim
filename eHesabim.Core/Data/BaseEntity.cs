using System;

namespace eHesabim.Core.Data {
    public abstract class BaseEntity<TId> : ReadOnlyEntity<TId> {
        public bool IsDeleted { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime? UpdatedDateTime { get; set; }
    }
}
