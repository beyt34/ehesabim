using System;

namespace eHesabim.Data {
    /// <summary>The base entity.</summary>
    /// <typeparam name="TId">t type id.</typeparam>
    public abstract class BaseEntity<TId> : ReadOnlyEntity<TId> {
        /// <summary>Gets or sets a value indicating whether is deleted.</summary>
        public bool IsDeleted { get; set; }

        /// <summary>Gets or sets the created date time.</summary>
        public DateTime CreatedDateTime { get; set; }

        /// <summary>Gets or sets the updated date time.</summary>
        public DateTime? UpdatedDateTime { get; set; }
    }
}
