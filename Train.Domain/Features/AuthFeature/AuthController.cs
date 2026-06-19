using Contracts;
using Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Base;
using Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train.Domain.Features.AuthFeature
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(request, cancellationToken);

            if (result.IsSuccess && result.Data is not null)
            {
                Response.AddAuthCookies(result.Data);

                var safeData = new LoginResponse
                {
                    UserName = result.Data.UserName,
                    RoleCode = result.Data.RoleCode
                };

                return Execute(Result<LoginResponse>.SetResponse(result.Code, result.Type, safeData));
            }

            return Execute(result);
        }

        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var result = await _authService.RefreshAsync(refreshToken, cancellationToken);

            if (result.IsSuccess && result.Data is not null)
            {
                Response.AddAuthCookies(result.Data);

                var safeData = new LoginResponse
                {
                    UserName = result.Data.UserName,
                    RoleCode = result.Data.RoleCode
                };
                return Execute(Result<LoginResponse>.SetResponse(result.Code, result.Type, safeData));
            }

            return Execute(result);
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refresh_token"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _authService.LogoutAsync(refreshToken, cancellationToken);
            }

            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            return Execute(Result<bool>.SetResponse(
                Shared.Constants.ConstantResponseCode.AuthLogoutSuccess,
                EnumRespType.Success,
                true
                ));
        }

        [Authorize]
        [HttpGet("Me")]
        public IActionResult GetCurrentUser()
        {
            var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "";
            var roleCode = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "";

            var safeData = new LoginResponse
            {
                UserName = userName,
                RoleCode = roleCode
            };

            return Execute(Result<LoginResponse>.SetResponse(
                Shared.Constants.ConstantResponseCode.AuthLoginSuccess,
                EnumRespType.Success,
                safeData
            ));
        }
    }
}
