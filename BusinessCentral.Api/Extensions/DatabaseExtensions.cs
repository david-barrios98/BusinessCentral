using Microsoft.EntityFrameworkCore;
using BusinessCentral.Infrastructure.Persistence.Adapters;

namespace BusinessCentral.Api.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<BusinessCentralDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        return services;
    }
}