using System.Data.Entity.ModelConfiguration;
using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class CityMap : EntityTypeConfiguration<City> {
        public CityMap() {
            ToTable("tblCity");
            HasKey(a => a.Id);
        }
    }
}
