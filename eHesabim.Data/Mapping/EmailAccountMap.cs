using System.Data.Entity.ModelConfiguration;

using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class EmailAccountMap : EntityTypeConfiguration<EmailAccount> {
        public EmailAccountMap() {
            ToTable("tblEmailAccount");
            HasKey(a => a.Id);
        }
    }
}
