using BusinessCentral.Application.DTOs.Common;
using System.Net;

namespace BusinessCentral.Api.Middleware;

/// <summary>
/// Enforce el canal de consumo (Backoffice vs Tenant App) por ruta.
/// - /api/v1/system/** => X-Client: backoffice
/// - /api/v1/secure/** => X-Client: tenant-app
/// </summary>
public sealed class ClientChannelMiddleware
{
    private readonly RequestDelegate _next;

    public ClientChannelMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        if (string.IsNullOrWhiteSpace(path))
        {
            await _next(context);
            return;
        }

        // Solo aplicamos en rutas API
        if (!path.StartsWith("/api/v1/", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var requiredClient =
            path.StartsWith("/api/v1/system/", StringComparison.OrdinalIgnoreCase) ? "backoffice" :
            path.StartsWith("/api/v1/secure/", StringComparison.OrdinalIgnoreCase) ? "tenant-app" :
            null;

        if (requiredClient is null)
        {
            await _next(context);
            return;
        }

        var client = (context.Request.Headers["X-Client"].ToString() ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(client))
        {
            // Si no llega, lo tratamos como mismatch (canal obligatorio) para evitar bypass.
            await DenyAsync(context, "X-Client es requerido para este endpoint.", HttpStatusCode.Forbidden);
            return;
        }

        if (!string.Equals(client, requiredClient, StringComparison.OrdinalIgnoreCase))
        {
            await DenyAsync(context, $"Canal inválido para este endpoint. Se requiere X-Client: {requiredClient}.", HttpStatusCode.Forbidden);
            return;
        }

        await _next(context);
    }

    private static Task DenyAsync(HttpContext context, string message, HttpStatusCode code)
    {
        context.Response.StatusCode = (int)code;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(ApiResponse<object>.Failure(message, (int)code));
    }
}

