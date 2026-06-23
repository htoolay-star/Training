using System.ComponentModel.DataAnnotations;

namespace Contracts.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "User name is required.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }

    //public class RefreshTokenRequest
    //{
    //    [Required]
    //    public string RefreshToken { get; set; } = string.Empty;
    //}

    //public class LogoutRequest
    //{
    //    [Required]
    //    public string RefreshToken { get; set; } = string.Empty;
    //}

    public class LoginResponse
    {
        public string UserName { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
    }

    public class AuthTokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiresAt { get; set; }
        public string TokenType { get; set; } = "Bearer";

        public string UserName { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
    }

}
