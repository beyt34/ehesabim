using System.ComponentModel.DataAnnotations;

namespace eHesabim.Data {
    public abstract class ReadOnlyEntity<T> {
        [Key]
        public virtual T Id { get; set; }
    }
}
