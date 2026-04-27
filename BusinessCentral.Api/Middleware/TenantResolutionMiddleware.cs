using BusinessCentral.Api.Services;
using BusinessCentral.Application.Ports.Outbound;
using System.Text.Json;

namespace BusinessCentral.Api.Middleware;

/// <summary>
/// Resuelve el tenant (CompanyId) de forma consistente y lo deja en un contexto scoped.
/// Estrategias:
/// - JWT claim "companyId"
/// - Headers "X-Company-Id" o "companyId" (compat)
/// - Subdominio (si hay SP de lookup)
/// - Login body (compat con tu contrato actual)
/// </summary>
public sealed class TenantResolutionMiddleware
{
    private const string HeaderCompanyId = "X-Company-Id";
    private const string LegacyHeaderCompanyId = "companyId";

    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext, ITenantRepository tenantRepository)
    {
        // 1) Claim
        var companyIdStr = context.User.FindFirst("companyId")?.Value;

        // 2) Headers
        if (string.IsNullOrWhiteSpace(companyIdStr))
        {
            companyIdStr = context.Request.Headers.TryGetValue(HeaderCompanyId, out var h1) && !string.IsNullOrWhiteSpace(h1)
                ? h1.ToString()
                : null;
        }

        if (string.IsNullOrWhiteSpace(companyIdStr))
        {
            companyIdStr = context.Request.Headers.TryGetValue(LegacyHeaderCompanyId, out var h2) && !string.IsNullOrWhiteSpace(h2)
                ? h2.ToString()
                : null;
        }

        // 3) Subdomain lookup (si aplica)
        if (string.IsNullOrWhiteSpace(companyIdStr))
        {
            var subdomain = TryGetSubdomain(context.Request.Host.Host);
            if (!string.IsNullOrWhiteSpace(subdomain))
            {
                tenantContext.Subdomain = subdomain;
                var companyIdFromSubdomain = await tenantRepository.GetCompanyIdBySubdomainAsync(subdomain);
                if (companyIdFromSubdomain is not null)
                {
                    tenantContext.CompanyId = companyIdFromSubdomain.Value;
                    await _next(context);
                    return;
                }
            }
        }

        // 4) Login body (compat)
        if (string.IsNullOrWhiteSpace(companyIdStr) && IsLoginRoute(context))
        {
            companyIdStr = await GetCompanyIdFromLoginBody(context);
        }

        if (int.TryParse(companyIdStr, out var companyId))
        {
            tenantContext.CompanyId = companyId;
        }

        await _next(context);
    }

    private static bool IsLoginRoute(HttpContext context)
        => context.Request.Path.Value?.Contains("/auth/login", StringComparison.OrdinalIgnoreCase) ?? false;

    private static async Task<string?> GetCompanyIdFromLoginBody(HttpContext context)
    {
        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, System.Text.Encoding.UTF8, false, 1024, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;

        if (string.IsNullOrWhiteSpace(body)) return null;

        try
        {
            using var jsonDoc = JsonDocument.Parse(body);
            if (jsonDoc.RootElement.TryGetProperty("userLogin", out var userLogin) &&
                userLogin.TryGetProperty("companyId", out var companyProp))
            {
                return companyProp.ValueKind == JsonValueKind.String
                    ? companyProp.GetString()
                    : companyProp.GetRawText();
            }
        }
        catch
        {
            // ignore
        }

        return null;
    }

    private static string? TryGetSubdomain(string host)
    {
        // No subdominio en localhost o ip
        if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase)) return null;
        if (host.Count(c => c == '.') < 2) return null;

        var parts = host.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length < 3) return null;

        return parts[0];
    }
}

