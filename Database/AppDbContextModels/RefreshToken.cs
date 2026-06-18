using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.AppDbContextModels
{
    public class RefreshToken : BaseEntity
    {
        public long AdminUserId { get; set; }
        public AdminUser? AdminUser { get; set; }

        public string TokenHash { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public string? ReplacedByTokenHash { get; set; }

        public bool IsActive => RevokedAt is null && ExpiresAt > Shared.Extensions.DateTimeHelper.CurrentMyanmarDateTime;
    }
}
