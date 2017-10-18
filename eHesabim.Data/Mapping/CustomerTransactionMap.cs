using System.Data.Entity.ModelConfiguration;
using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class CustomerTransactionMap : EntityTypeConfiguration<CustomerTransaction> {
        public CustomerTransactionMap() {
            ToTable("tblCustomerTransaction");
            HasKey(a => a.Id);

            HasOptional(a => a.Parent).WithMany().HasForeignKey(x => x.ParentId).WillCascadeOnDelete(false);
            HasRequired(a => a.Customer).WithMany().HasForeignKey(x => x.CustomerId).WillCascadeOnDelete(false);
        }
    }
}
