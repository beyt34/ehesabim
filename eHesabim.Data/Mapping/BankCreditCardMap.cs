using System.Data.Entity.ModelConfiguration;
using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class BankCreditCardMap : EntityTypeConfiguration<BankCreditCard> {
        public BankCreditCardMap() {
            ToTable("tblBankCreditCard");
            HasKey(a => a.Id);

            HasRequired(a => a.Bank).WithMany().HasForeignKey(x => x.BankId).WillCascadeOnDelete(false);
        }
    }
}
