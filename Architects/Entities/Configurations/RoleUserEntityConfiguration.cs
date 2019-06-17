using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessControl.Architects.Entities.Configurations
{
    public class RoleUserEntityConfiguration : IEntityTypeConfiguration<RoleUserEntity>
    {
        public void Configure(EntityTypeBuilder<RoleUserEntity> builder)
        {
            builder.ToTable("RoleUser");
            builder.HasKey(v => new { v.RoleId, v.UserId });
        }
    }
}
