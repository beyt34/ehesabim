using System.Data.Entity.ModelConfiguration;
using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class ExpenseMap : EntityTypeConfiguration<Expense> {
        public ExpenseMap() {
            ToTable("tblExpense");
            HasKey(a => a.Id);

            HasRequired(a => a.Type).WithMany().HasForeignKey(x => x.TypeId).WillCascadeOnDelete(false);
            HasRequired(a => a.ExpenseStore).WithMany().HasForeignKey(x => x.ExpenseStoreId).WillCascadeOnDelete(false);
            HasRequired(a => a.ExpenseGroup).WithMany().HasForeignKey(x => x.ExpenseGroupId).WillCascadeOnDelete(false);
            HasOptional(a => a.BankCreditCard).WithMany().HasForeignKey(x => x.BankCreditCardId).WillCascadeOnDelete(false);
            HasOptional(a => a.BankCreditCardPeriod).WithMany().HasForeignKey(x => x.BankCreditCardPeriodId).WillCascadeOnDelete(false);
            HasOptional(a => a.BankCreditSub).WithMany().HasForeignKey(x => x.BankCreditSubId).WillCascadeOnDelete(false);
        }
    }
}
