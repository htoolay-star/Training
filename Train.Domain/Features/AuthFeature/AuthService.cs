using Contracts;
using Contracts.Auth;
using Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Auth;
using Shared.Constants;
using Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train.Domain.Features.AuthFeature
{
    public sealed class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwt;
        private readonly ILogger<AuthService> _logger;
        private readonly int _maxFailedAttempts;
        private readonly int _lockoutMinutes;

        public AuthService(
            AppDbContext db,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwt,
            ILogger<AuthService> logger,
            IOptions<AuthOptions> authOptions)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _jwt = jwt;
            _logger = logger;
            _maxFailedAttempts = authOptions.Value.MaxFailedAttempts;
            _lockoutMinutes = authOptions.Value.LockoutMinutes;
        }

        public async Task<Result<AuthTokenResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            var now = DateTimeHelper.CurrentMyanmarDateTime;

            var user = await _db.AdminUsers
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserName == request.UserName, cancellationToken)
                .ConfigureAwait(false);

            if (user is null)
                return Result<AuthTokenResponse>.SetResponse(ConstantResponseCode.AuthInvalidCredentials, EnumRespType.BadRequest);

            if (user.LockoutEndAt.HasValue && user.LockoutEndAt.Value > now)
                return Result<AuthTokenResponse>.SetResponse(ConstantResponseCode.AuthAccountLocked, EnumRespType.BadRequest);

            if (!user.IsActive)
                return Result<AuthTokenResponse>.SetResponse(ConstantResponseCode.AuthAccountInactive, EnumRespType.BadRequest);

            if (!_passwordHasher.Verify(user.PasswordHash, request.Password))
            {
                user.AccessFailedCount++;
                var justLocked = user.AccessFailedCount >= _maxFailedAttempts;
                if (justLocked)
                {
                    user.LockoutEndAt = now.AddMinutes(_lockoutMinutes);
                    user.AccessFailedCount = 0;
                }
                await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return Result<AuthTokenResponse>.SetResponse(
                    justLocked ? ConstantResponseCode.AuthAccountLocked : ConstantResponseCode.AuthInvalidCredentials,
                    EnumRespType.BadRequest);
            }

            // Success — reset failure state, stamp login, issue tokens.
            user.AccessFailedCount = 0;
            user.LockoutEndAt = null;
            user.LastLoginAt = now;

            // save all data in IssueTokensAsync
            var roleCode = user.Role?.Code ?? string.Empty;
            var response = await IssueTokensAsync(user.Id, user.UserName, roleCode, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("User {UserName} signed in.", user.UserName);
            return Result<AuthTokenResponse>.SetResponse(ConstantResponseCode.AuthLoginSuccess, EnumRespType.Success, response);
        }

        public async Task<Result<AuthTokenResponse>> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var hash = _jwt.HashRefreshToken(refreshToken);

            var token = await _db.RefreshTokens
                .Include(t => t.AdminUser)!
                .ThenInclude(u => u!.Role)
                .FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken)
                .ConfigureAwait(false);

            if (token is null)
                return Result<AuthTokenResponse>.SetResponse(ConstantResponseCode.AuthInvalidToken, EnumRespType.BadRequest);

            // Token reuse detection — if a revoked token is presented, revoke all user tokens
            if (token.RevokedAt is not null)
            {
                await _db.RefreshTokens
                    .Where(t => t.AdminUserId == token.AdminUserId && t.RevokedAt == null)
                    .ExecuteUpdateAsync(
                        setter => setter.SetProperty(t => t.RevokedAt, DateTimeHelper.CurrentMyanmarDateTime),
                        cancellationToken)
                    .ConfigureAwait(false);

                return Result<AuthTokenResponse>.SetResponse(ConstantResponseCode.AuthInvalidToken, EnumRespType.BadRequest);
            }

            if (!token.IsActive || token.AdminUser is null)
                return Result<AuthTokenResponse>.SetResponse(ConstantResponseCode.AuthInvalidToken, EnumRespType.BadRequest);

            var user = token.AdminUser;
            if (!user.IsActive)
                return Result<AuthTokenResponse>.SetResponse(ConstantResponseCode.AuthAccountInactive, EnumRespType.BadRequest);

            var roleCode = user.Role?.Code ?? string.Empty;

            // Rotate: revoke the presented token, issue a new pair.
            var response = await IssueTokensAsync(user.Id, user.UserName, roleCode, cancellationToken, rotatedFrom: token)
                .ConfigureAwait(false);

            return Result<AuthTokenResponse>.SetResponse(ConstantResponseCode.AuthLoginSuccess, EnumRespType.Success, response);
        }

        public async Task<Result<bool>> LogoutAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var hash = _jwt.HashRefreshToken(refreshToken);
            var token = await _db.RefreshTokens
                .FirstOrDefaultAsync(t => t.TokenHash == hash && t.RevokedAt == null, cancellationToken)
                .ConfigureAwait(false);

            if (token is not null)
            {
                token.RevokedAt = DateTimeHelper.CurrentMyanmarDateTime;
                await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            // Idempotent — always report success.
            return Result<bool>.SetResponse(ConstantResponseCode.AuthLogoutSuccess, EnumRespType.Success, true);
        }

        private async Task<AuthTokenResponse> IssueTokensAsync(
            long userId, string userName, string roleCode, CancellationToken cancellationToken,
            RefreshToken? rotatedFrom = null)
        {
            var access = _jwt.CreateAccessToken(userId, userName, roleCode);
            var refresh = _jwt.CreateRefreshToken();

            if (rotatedFrom is not null)
            {
                rotatedFrom.RevokedAt = DateTimeHelper.CurrentMyanmarDateTime;
                rotatedFrom.ReplacedByTokenHash = refresh.TokenHash;
            }

            _db.RefreshTokens.Add(new RefreshToken
            {
                AdminUserId = userId,
                TokenHash = refresh.TokenHash,
                ExpiresAt = refresh.ExpiresAt
            });

            await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new AuthTokenResponse
            {
                AccessToken = access.Token,
                AccessTokenExpiresAt = access.ExpiresAt,
                RefreshToken = refresh.RawToken,
                RefreshTokenExpiresAt = refresh.ExpiresAt,
                TokenType = "Bearer",
                UserName = userName,
                RoleCode = roleCode
            };
        }
    }
}
