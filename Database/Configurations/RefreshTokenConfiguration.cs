using Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> b)
        {
            b.ToTable("refresh_token");
            b.HasKey(x => x.Id);

            b.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();
            b.Property(x => x.ReplacedByTokenHash).HasMaxLength(128);

            // IsActive is computed in C#; don't map it to a column.
            b.Ignore(x => x.IsActive);

            b.HasIndex(x => x.TokenHash);
            b.HasIndex(x => x.AdminUserId);

            b.HasOne(x => x.AdminUser)
                .WithMany()
                .HasForeignKey(x => x.AdminUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
