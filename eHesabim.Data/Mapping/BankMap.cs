using System.Data.Entity.ModelConfiguration;
using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class BankMap : EntityTypeConfiguration<Bank> {
        public BankMap() {
            ToTable("tblBank");
            HasKey(a => a.Id);
        }
    }
}
