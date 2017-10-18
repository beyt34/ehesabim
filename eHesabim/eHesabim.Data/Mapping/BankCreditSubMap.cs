using System.Data.Entity.ModelConfiguration;
using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class BankCreditSubMap : EntityTypeConfiguration<BankCreditSub> {
        public BankCreditSubMap() {
            ToTable("tblBankCreditSub");
            HasKey(a => a.Id);
            
            HasRequired(a => a.BankCredit).WithMany().HasForeignKey(x => x.BankCreditId).WillCascadeOnDelete(false);
        }
    }
}
