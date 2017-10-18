using System.Data.Entity.ModelConfiguration;

using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class EmailQueueMap : EntityTypeConfiguration<EmailQueue> {
        public EmailQueueMap() {
            ToTable("tblEmailQueue");
            HasKey(a => a.Id);

            HasOptional(qe => qe.EmailAccount).WithMany().HasForeignKey(qe => qe.EmailAccountId);
        }
    }
}
