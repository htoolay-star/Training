namespace Database.AppDbContextModels
{
    public class AdminUser : AuditableEntity
    {
        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public long RoleId { get; set; }

        public Role? Role { get; set; }

        public bool IsActive { get; set; } = true;

        public int AccessFailedCount { get; set; }

        public DateTime? LockoutEndAt { get; set; }

        public DateTime? LastLoginAt { get; set; }
    }
}
