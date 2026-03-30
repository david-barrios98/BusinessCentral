using BusinessCentral.Api.Middleware;

namespace BusinessCentral.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
        return app;
    }
}