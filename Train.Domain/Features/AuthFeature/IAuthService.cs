using Contracts;
using Contracts.Auth;

namespace Train.Domain.Features.AuthFeature
{
    public interface IAuthService
    {
        Task<Result<AuthTokenResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
        Task<Result<AuthTokenResponse>> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
        Task<Result<bool>> LogoutAsync(string refreshToken, CancellationToken cancellationToken);
    }
}
