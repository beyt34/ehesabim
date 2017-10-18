using System.Data.Entity.ModelConfiguration;
using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class BankCreditCardPaymentMap : EntityTypeConfiguration<BankCreditCardPayment> {
        public BankCreditCardPaymentMap() {
            ToTable("tblBankCreditCardPayment");
            HasKey(a => a.Id);

            HasRequired(a => a.BankCreditCard).WithMany().HasForeignKey(x => x.BankCreditCardId).WillCascadeOnDelete(false);
        }
    }
}
