using System.Data.Entity.ModelConfiguration;

using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class EmailTemplateMap : EntityTypeConfiguration<EmailTemplate> {
        public EmailTemplateMap() {
            ToTable("tblEmailTemplate");
            HasKey(a => a.Id);

            HasRequired(a => a.EmailAccount).WithMany().HasForeignKey(x => x.EmailAccountId).WillCascadeOnDelete(false);
        }
    }
}
