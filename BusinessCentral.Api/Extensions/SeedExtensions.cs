using BusinessCentral.Infrastructure.Persistence.Adapters;
using BusinessCentral.Infrastructure.Seed;

namespace BusinessCentral.Api.Extensions;

public static class SeedExtensions
{
    public static async Task RunSeedIfNeeded(this WebApplication app, IConfiguration config)
    {
        if (!config.GetValue<bool>("RunSeed")) return;

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BusinessCentralDbContext>();

        var seedDemoData = config.GetValue<bool>("RunSeedDemoData");
        await DbInitializer.SeedAsync(context, seedDemoData: seedDemoData);
    }
}