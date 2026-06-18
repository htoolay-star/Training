using Microsoft.AspNetCore.Http;
using Shared.Constants;
using Shared.Extensions;
using System.Security.Claims;


namespace Shared.Base
{
    public class BaseService : IBaseService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;
        private HttpRequest? Request => _httpContextAccessor.HttpContext?.Request;

        public long? UserId => User?.FindFirst(ConstantClaimCode.UserId)?.Value.ToLong();

        public string? UserName => User?.FindFirst(ConstantClaimCode.UserName)?.Value;

        public string? RoleCode => User?.FindFirst(ConstantClaimCode.RoleCode)?.Value;

        public DateTime CurrentMyanmarDateTime => DateTimeHelper.CurrentMyanmarDateTime;

        public string? AcceptLanguage => Request?.Headers["Accept-Language"].FirstOrDefault();

        public string? AccessToken
        {
            get
            {
                var authHeader = Request?.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(authHeader) &&
                    authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    return authHeader["Bearer ".Length..].Trim();
                }
                return null;
            }
        }
    }
}
