using Contracts;
using Contracts.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train.Domain.Features.AuthFeature
{
    public interface IAuthService
    {
        Task<Result<AuthTokenResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
        Task<Result<AuthTokenResponse>> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken);
        Task<Result<bool>> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken);
    }
}
