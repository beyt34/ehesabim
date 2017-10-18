using System.Data.Entity.ModelConfiguration;

using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class BankAccountTransactionMap : EntityTypeConfiguration<BankAccountTransaction> {
        public BankAccountTransactionMap() {
            ToTable("tblBankAccountTransaction");
            HasKey(a => a.Id);

            HasRequired(a => a.BankAccount).WithMany().HasForeignKey(x => x.BankAccountId).WillCascadeOnDelete(false);
            HasOptional(a => a.Related).WithMany().HasForeignKey(x => x.RelatedId).WillCascadeOnDelete(false);
            HasOptional(a => a.CustomerTransaction).WithMany().HasForeignKey(x => x.CustomerTransactionId).WillCascadeOnDelete(false);
            HasOptional(a => a.BankCreditCardPayment).WithMany(b => b.BankAccountTransactions).HasForeignKey(x => x.BankCreditCardPaymentId).WillCascadeOnDelete(false);
            HasOptional(a => a.BankCreditSub).WithMany().HasForeignKey(x => x.BankCreditSubId).WillCascadeOnDelete(false);
            HasOptional(a => a.Expense).WithMany().HasForeignKey(x => x.ExpenseId).WillCascadeOnDelete(false);
        }
    }
}
