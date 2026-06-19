using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Auth
{
    public static class AuthServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthPrimitives(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.SectionName));
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();
            return services;
        }
    }
}
