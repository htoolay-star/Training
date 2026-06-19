namespace Shared.Auth
{
    public sealed record AccessTokenResult(string Token, DateTime ExpiresAt);

    public sealed record RefreshTokenResult(string RawToken, string TokenHash, DateTime ExpiresAt);

    public interface IJwtTokenService
    {
        AccessTokenResult CreateAccessToken(long userId, string userName, string roleCode);

        RefreshTokenResult CreateRefreshToken();

        string HashRefreshToken(string rawToken);
    }
}
