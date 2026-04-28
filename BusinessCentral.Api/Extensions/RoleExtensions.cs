using BusinessCentral.Api.Middleware;
using BusinessCentral.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;

namespace BusinessCentral.Api.Extensions
{
    public static class RoleExtensions
    {
        public static IServiceCollection AddRoleExtensions(this IServiceCollection services)
        {
            // Registrar el AuthorizationHandler y la policy "SystemRole"
            services.AddScoped<IAuthorizationHandler, BusinessCentral.Infrastructure.Security.SystemRoleHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("SystemRole", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new SystemRoleRequirement());
                });
            });

            services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationMiddlewareResultHandler>();

            return services;
        }
    }
}
