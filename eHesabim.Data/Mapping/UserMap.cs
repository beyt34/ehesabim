using System.Data.Entity.ModelConfiguration;
using eHesabim.Data.Domain;

namespace eHesabim.Data.Mapping {
    public class UserMap : EntityTypeConfiguration<User> {
        public UserMap() {
            ToTable("tblUser");
            HasKey(a => a.Id);
        }
    }
}
