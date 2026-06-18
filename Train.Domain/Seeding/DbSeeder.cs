using Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train.Domain.Seeding
{
    public sealed class DbSeeder : IDbSeeder
    {
        private const string BootstrapUserName = "admin";
        private const string BootstrapEmail = "superadmin@globalnet.local";
        private const string BootstrapPassword = "123@Test.com";

        private readonly AppDbContext _db;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<DbSeeder> _logger;

        public DbSeeder(AppDbContext db, IPasswordHasher passwordHasher, ILogger<DbSeeder> logger)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            var anyRole = await _db.Roles
                .IgnoreQueryFilters()
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!anyRole)
            {
                _db.Roles.Add(new Role
                {
                    Code = "SuperAdmin",
                    Name = "Super Admin",
                    IsSystem = true
                });
                await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            var anyAdmin = await _db.AdminUsers
                .IgnoreQueryFilters()
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false);

            if (anyAdmin)
                return;

            _db.AdminUsers.Add(new AdminUser
            {
                UserName = BootstrapUserName,
                Email = BootstrapEmail,
                PasswordHash = _passwordHasher.Hash(BootstrapPassword),
                RoleId = 1,
                IsActive = true
            });

            await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogWarning(
                "Seeded bootstrap admin '{UserName}'. CHANGE THE DEFAULT PASSWORD after first login.",
                BootstrapUserName);
        }
    }

}
