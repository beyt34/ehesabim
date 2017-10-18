using System.Data.Entity.ModelConfiguration;
using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class ExpenseStoreMap : EntityTypeConfiguration<ExpenseStore> {
        public ExpenseStoreMap() {
            ToTable("tblExpenseStore");
            HasKey(a => a.Id);
        }
    }
}
