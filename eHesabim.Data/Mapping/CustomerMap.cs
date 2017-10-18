using System.Data.Entity.ModelConfiguration;
using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class CustomerMap : EntityTypeConfiguration<Customer> {
        public CustomerMap() {
            ToTable("tblCustomer");
            HasKey(a => a.Id);
        }
    }
}
