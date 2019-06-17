using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace AccessControl.Architects.Entities.Configurations
{
    public class PermissionEntityConfiguration : IEntityTypeConfiguration<PermissionEntity>
    {
        public void Configure(EntityTypeBuilder<PermissionEntity> builder)
        {
            builder.ToTable("Permission");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.ParentId).IsRequired(true);
            builder.Property(v => v.Type).IsRequired(true);
            builder.Property(v => v.Code).HasMaxLength(50).IsRequired(true);
            builder.Property(v => v.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(v => v.Icon).HasMaxLength(50).IsRequired(true);
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
