using System.Data.Entity.ModelConfiguration;
using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class BankAccountMap : EntityTypeConfiguration<BankAccount> {
        public BankAccountMap() {
            ToTable("tblBankAccount");
            HasKey(a => a.Id);

            HasRequired(a => a.Type).WithMany().HasForeignKey(x => x.TypeId).WillCascadeOnDelete(false);
            HasOptional(a => a.Bank).WithMany().HasForeignKey(x => x.BankId).WillCascadeOnDelete(false);
        }
    }
}
