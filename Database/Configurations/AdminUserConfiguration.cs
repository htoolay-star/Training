using Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    public class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
    {
        public void Configure(EntityTypeBuilder<AdminUser> b)
        {
            b.ToTable("admin_user");
            b.HasKey(x => x.Id);

            b.Property(x => x.UserName).HasMaxLength(100).IsRequired();
            b.Property(x => x.Email).HasMaxLength(128).IsRequired();
            b.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();

            b.HasIndex(x => x.UserName).IsUnique();
            b.HasIndex(x => x.Email);

            b.HasOne(x => x.Role)
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
