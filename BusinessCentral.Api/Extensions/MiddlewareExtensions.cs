using BusinessCentral.Api.Middleware;
using BusinessCentral.Api.Services;
using BusinessCentral.Application.Services;

namespace BusinessCentral.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IServiceCollection AddTenantContext(this IServiceCollection services)
    {
        services.AddScoped<ITenantContext, TenantContext>();
        return services;
    }

    public static IApplicationBuilder UseGlobalMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<ClientChannelMiddleware>();
        app.UseMiddleware<TenantResolutionMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseMiddleware<TenantSubscriptionMiddleware>();

        return app;
    }
}