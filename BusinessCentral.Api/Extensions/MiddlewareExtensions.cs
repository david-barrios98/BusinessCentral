using BusinessCentral.Api.Middleware;
using BusinessCentral.Api.Security;
using Microsoft.AspNetCore.Authorization;

namespace BusinessCentral.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseMiddleware<TenantSubscriptionMiddleware>();

        return app;
    }
}