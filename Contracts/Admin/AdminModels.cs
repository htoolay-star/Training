using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Admin
{
    public class RoleListDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsSystem { get; set; }
    }

    public class RoleDetailDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsSystem { get; set; }
    }

    public class CreateRoleRequest
    {
        [Required] public string Code { get; set; } = string.Empty;
        [Required] public string Name { get; set; } = string.Empty;
    }

    public class UpdateRoleRequest
    {
        [Required] public long Id { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
    }

    // ---------------- Admin user ----------------
    public class AdminUserListDto
    {
        public long Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    public class AdminUserDetailDto
    {
        public long Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long RoleId { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateAdminUserRequest
    {
        [Required] public string UserName { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, MinLength(8)] public string Password { get; set; } = string.Empty;
        [Required] public long RoleId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateAdminUserRequest
    {
        [Required] public long Id { get; set; }
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public long RoleId { get; set; }
        public bool IsActive { get; set; }
    }

    public class ResetPasswordRequest
    {
        [Required] public long Id { get; set; }
        [Required, MinLength(8)] public string NewPassword { get; set; } = string.Empty;
    }
}
