using System.Data.Entity.ModelConfiguration;
using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class CountyMap : EntityTypeConfiguration<County> {
        public CountyMap() {
            ToTable("tblCounty");
            HasKey(a => a.Id);

            HasRequired(a => a.City).WithMany().HasForeignKey(x => x.CityId).WillCascadeOnDelete(false);
        }
    }
}
