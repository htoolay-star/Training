using Database.AppDbContextModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Auth;
using Shared.Base;
using Train.Domain.Features.AdminUserFeature;
using Train.Domain.Features.AuthFeature;
using Train.Domain.Features.RoleFeature;
using Train.Domain.Seeding;

namespace Train.Domain
{
    public static class FeatureManager
    {
        public static WebApplicationBuilder AddSmsDomain(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var config = builder.Configuration;

            var connectionString = config.GetConnectionString("Default")
                ?? throw new InvalidOperationException("Missing ConnectionStrings:Default.");
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // ---- Framework / ambient services ----
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddScoped<IBaseService, BaseService>();

            services.AddAuthPrimitives(config);

            // ---- Feature services + background job ----
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAdminUserService, AdminUserService>();

            // ---- Startup data seeding (bootstrap admin) ----
            services.AddScoped<IDbSeeder, DbSeeder>();

            return builder;
        }
    }
}
