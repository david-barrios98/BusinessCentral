using Microsoft.EntityFrameworkCore;
using BusinessCentral.Infrastructure.Persistence.Adapters;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BusinessCentral.Api.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<BusinessCentralDbContext>(options =>
            options
                .UseSqlServer(config.GetConnectionString("DefaultConnection"))
                // Permite correr Migrate/Seed en dev aunque el modelo cambie.
                // Idealmente: generar migración y mantener snapshot sincronizado.
                .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

        return services;
    }
}