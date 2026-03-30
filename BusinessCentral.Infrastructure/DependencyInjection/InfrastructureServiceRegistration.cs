using Microsoft.Extensions.DependencyInjection;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Persistence.Repositories;
using BusinessCentral.Infrastructure.Security;
using BusinessCentral.Shared.Helper;

namespace BusinessCentral.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<ILoginRepository, LoginRepository>();

            // Services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IHashPasswordService, HashPasswordService>();
            services.AddSingleton<JwtService>();


            return services;
        }
    }
}