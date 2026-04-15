using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.ExternalServices;
using BusinessCentral.Infrastructure.Persistence.Adapters;
using BusinessCentral.Infrastructure.Persistence.Configuration;
using BusinessCentral.Infrastructure.Persistence.Repositories;
using BusinessCentral.Infrastructure.Security;
using BusinessCentral.Shared.Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessCentral.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<FailedLoginOptions>(config.GetSection("FailedLoginOptions"));

            // Repositories
            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IUserSessionRepository, UserSessionRepository>();
            services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();

            // Services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IHashPasswordService, HashPasswordService>();
            services.AddSingleton<JwtService>();
            services.AddSingleton<IFailedLoginAttemptService, FailedLoginAttemptService>();
            services.AddScoped<IEmailService, SmtpEmailService>();

            return services;
        }
    }
}