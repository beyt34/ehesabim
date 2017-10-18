using System.ComponentModel.DataAnnotations;

namespace eHesabim.Core.Data {
    public abstract class ReadOnlyEntity<T> {
        [Key]
        public virtual T Id { get; set; }
    }
}
