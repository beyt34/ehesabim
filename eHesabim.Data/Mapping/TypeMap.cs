using System.Data.Entity.ModelConfiguration;
using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class TypeMap : EntityTypeConfiguration<Type> {
        public TypeMap() {
            ToTable("tblType");
            HasKey(a => a.Id);
        }
    }
}
