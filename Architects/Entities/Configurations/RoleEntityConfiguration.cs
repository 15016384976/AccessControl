using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace AccessControl.Architects.Entities.Configurations
{
    public class RoleEntityConfiguration : IEntityTypeConfiguration<RoleEntity>
    {
        public void Configure(EntityTypeBuilder<RoleEntity> builder)
        {
            builder.ToTable("Role");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(v => v.Available).IsRequired(true);
            builder.Property(v => v.Description).HasMaxLength(255).IsRequired(false);
            builder.Property(v => v.CreateBy).IsRequired(true);
            builder.Property(v => v.CreateTime).IsRequired(true);
            builder.Property(v => v.UpdateBy).IsRequired(false);
            builder.Property(v => v.UpdateTime).IsRequired(false);
            builder.Property<Guid?>("DeleteBy").IsRequired(false);
            builder.Property<DateTime?>("DeleteTime").IsRequired(false);
            builder.HasQueryFilter(v => EF.Property<Guid?>(v, "DeleteBy") == null && EF.Property<Guid?>(v, "DeleteTime") == null);
        }
    }
}
