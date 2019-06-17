using AccessControl.Architects.Entities.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AccessControl.Architects.Entities.Contexts
{
    public class DatabaseContext : DbContext
    {
        private readonly IClaimService _claimService;

        public DatabaseContext(DbContextOptions<DatabaseContext> options, IClaimService claimService) : base(options)
        {
            _claimService = claimService;
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            Before(_claimService.SubjectId);
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();
            Before(_claimService.SubjectId);
            return base.SaveChangesAsync(cancellationToken);
        }

        private void Before(string subjectId)
        {
            foreach (var item in ChangeTracker.Entries<IEntity>()
                .Where(v => v.State == EntityState.Added || v.State == EntityState.Modified || v.State == EntityState.Deleted))
            {
                var sid = subjectId == null ? Guid.Empty : Guid.Parse(subjectId);
                var now = DateTime.Now;
                switch (item.State)
                {
                    case EntityState.Added:
                        item.CurrentValues["CreateBy"] = sid;
                        item.CurrentValues["CreateTime"] = now;
                        break;
                    case EntityState.Modified:
                        if (item.CurrentValues["DeleteBy"] == null && item.CurrentValues["DeleteTime"] == null)
                        {
                            item.CurrentValues["UpdateBy"] = sid;
                            item.CurrentValues["UpdateTime"] = now;
                        }
                        break;
                    case EntityState.Deleted:
                        item.State = EntityState.Modified;
                        item.CurrentValues["DeleteBy"] = sid;
                        item.CurrentValues["DeleteTime"] = now;
                        break;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new RoleEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new RoleUserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionEntityConfiguration());
            modelBuilder.ApplyConfiguration(new RolePermissionEntityConfiguration());
        }

        public DbSet<RoleEntity> RoleEntities { get; set; }

        public DbSet<UserEntity> UserEntities { get; set; }

        public DbSet<RoleUserEntity> RoleUserEntities { get; set; }

        public DbSet<PermissionEntity> PermissionEntities { get; set; }

        public DbSet<RolePermissionEntity> RolePermissionEntities { get; set; }
    }
}
