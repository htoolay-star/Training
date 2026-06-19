using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shared.Base;
using Shared.Extensions;
using System.Linq.Expressions;
using System.Reflection;

namespace Database.AppDbContextModels
{
    public class AppDbContext : DbContext
    {
        private readonly IBaseService? _currentUser;

        public AppDbContext(DbContextOptions<AppDbContext> options, IBaseService? currentUser = null)
            : base(options)
        {
            _currentUser = currentUser;
        }

        public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (!typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType)) continue;

                var param = Expression.Parameter(entityType.ClrType, "e");
                var body = Expression.Not(Expression.Property(param, nameof(AuditableEntity.IsDeleted)));
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(body, param));
            }

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            StampAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            StampAuditFields();
            return base.SaveChanges();
        }

        private void StampAuditFields()
        {
            var now = DateTimeHelper.CurrentMyanmarDateTime;
            var userId = _currentUser?.UserId;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        if (entry.Entity is AuditableEntity addedAudit)
                            addedAudit.CreatedBy = userId;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        if (entry.Entity is AuditableEntity modifiedAudit)
                            modifiedAudit.UpdatedBy = userId;
                        break;

                    case EntityState.Deleted:

                        if (entry.Entity is AuditableEntity deletedAudit)
                        {
                            entry.State = EntityState.Modified;
                            deletedAudit.IsDeleted = true;
                            deletedAudit.UpdatedAt = now;
                            deletedAudit.UpdatedBy = userId;
                        }
                        break;
                }
            }
        }
    }
}
