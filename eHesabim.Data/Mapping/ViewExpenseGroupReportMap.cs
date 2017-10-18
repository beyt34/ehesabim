using System.Data.Entity.ModelConfiguration;
using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class ViewExpenseGroupReportMap : EntityTypeConfiguration<ViewExpenseGroupReport> {
        public ViewExpenseGroupReportMap() {
            ToTable("vwExpenseGroupReport");
            HasKey(a => a.Id);

            Property(a => a.Id).HasColumnName("RowId");
        }
    }
}
