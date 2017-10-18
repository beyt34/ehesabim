using System.Data.Entity.ModelConfiguration;
using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class BankCreditCardPeriodMap : EntityTypeConfiguration<BankCreditCardPeriod> {
        public BankCreditCardPeriodMap() {
            ToTable("tblBankCreditCardPeriod");
            HasKey(a => a.Id);

            HasRequired(a => a.BankCreditCard).WithMany().HasForeignKey(x => x.BankCreditCardId).WillCascadeOnDelete(false);
        }
    }
}
