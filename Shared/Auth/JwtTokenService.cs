using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Constants;
using Shared.Extensions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Auth
{
    public sealed class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _options;
        private readonly SigningCredentials _signingCredentials;

        public JwtTokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;

            if (string.IsNullOrWhiteSpace(_options.SigningKey) ||
                Encoding.UTF8.GetByteCount(_options.SigningKey) < 32)
                throw new InvalidOperationException(
                    "Jwt:SigningKey is missing or shorter than 32 bytes. Configure it via env/secret.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
            _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        public AccessTokenResult CreateAccessToken(long userId, string userName, string roleCode)
        {
            var lifetime = TimeSpan.FromMinutes(_options.AccessTokenMinutes);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new Claim(ConstantClaimCode.UserId, userId.ToString()),
            new Claim(ConstantClaimCode.UserName, userName),
            new Claim(ConstantClaimCode.RoleCode, roleCode)
        };

            // JWT exp/nbf are UTC by spec — use UtcNow here for correct validation. (DB datetime
            // fields use Myanmar time; this is a protocol value, not a stored field.)
            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.Add(lifetime),
                signingCredentials: _signingCredentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            // Return expiry in Myanmar time for display/clients.
            return new AccessTokenResult(jwt, DateTimeHelper.CurrentMyanmarDateTime.Add(lifetime));
        }

        public RefreshTokenResult CreateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            var raw = Base64UrlEncode(bytes);
            var expiresAt = DateTimeHelper.CurrentMyanmarDateTime.AddDays(_options.RefreshTokenDays);
            return new RefreshTokenResult(raw, HashRefreshToken(raw), expiresAt);
        }

        public string HashRefreshToken(string rawToken)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
            return Convert.ToHexString(hash); // uppercase hex
        }

        private static string Base64UrlEncode(byte[] bytes) =>
            Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

}
