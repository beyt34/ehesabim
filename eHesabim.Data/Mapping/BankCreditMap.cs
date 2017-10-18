using System.Data.Entity.ModelConfiguration;
using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class BankCreditMap : EntityTypeConfiguration<BankCredit> {
        public BankCreditMap() {
            ToTable("tblBankCredit");
            HasKey(a => a.Id);

            HasRequired(a => a.Bank).WithMany().HasForeignKey(x => x.BankId).WillCascadeOnDelete(false);
        }
    }
}
