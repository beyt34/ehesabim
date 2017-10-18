using System.Data.Entity.ModelConfiguration;
using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class ExpenseGroupMap : EntityTypeConfiguration<ExpenseGroup> {
        public ExpenseGroupMap() {
            ToTable("tblExpenseGroup");
            HasKey(a => a.Id);
        }
    }
}
