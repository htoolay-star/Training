using Contracts.Auth;
using Microsoft.AspNetCore.Http;

namespace Shared.Extensions
{
    public static class HttpResponseExtensions
    {
        public static void AddAuthCookies(this HttpResponse response, AuthTokenResponse tokens)
        {
            var accessOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = tokens.AccessTokenExpiresAt
            };
            response.Cookies.Append("access_token", tokens.AccessToken, accessOptions);

            var refreshOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = tokens.RefreshTokenExpiresAt
            };
            response.Cookies.Append("refresh_token", tokens.RefreshToken, refreshOptions);
        }

        public static void DeleteAuthCookies(this HttpResponse response)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            };
            response.Cookies.Delete("access_token", options);
            response.Cookies.Delete("refresh_token", options);
        }
    }
}
