using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace AccessControl.Architects.Entities.Configurations
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("User");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Account).HasMaxLength(50).IsRequired(true);
            builder.Property(v => v.Password).HasMaxLength(255).IsRequired(true);
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
