using BusinessCentral.Application.Constants;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using System.Net;
using BusinessCentral.Api.Services;
using BusinessCentral.Application.Services;

namespace BusinessCentral.Api.Middleware
{
    public class TenantSubscriptionMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantSubscriptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ISubscriptionRepository subscriptionService,
            ICompanyModuleRepository companyModuleRepository,
            ITenantContext tenantContext)
        {
            // Si no hay tenant, es ruta pública (ej: health) o petición sin contexto.
            if (tenantContext.CompanyId is null)
            {
                await _next(context);
                return;
            }

            var endpoint = context.GetEndpoint();
            var requiredModule = endpoint?.Metadata.GetMetadata<RequiresModuleAttribute>()?.ModuleName;
            var requiredPermission = endpoint?.Metadata.GetMetadata<RequiresPermissionAttribute>()?.Permission;

            // Si el endpoint requiere módulo, verificamos que esté habilitado para la compañía (parametrización).
            if (!string.IsNullOrWhiteSpace(requiredModule))
            {
                var enabled = await companyModuleRepository.IsCompanyModuleEnabledAsync(
                    tenantContext.CompanyId.Value,
                    requiredModule
                );

                if (!enabled)
                {
                    await HandleBlockedAccessAsync(context, AccessResult.ModuleNotIncluded);
                    return;
                }
            }

            // VALIDACIÓN CRÍTICA: Aquí es donde el SP verifica si la empresa está activa/paga
            var accessStatus = await subscriptionService.CheckAccessAsync(tenantContext.CompanyId.Value, requiredModule);

            if (accessStatus == AccessResult.Success)
            {
                await _next(context);
                return;
            }

            // 3er nivel (RBAC): si el endpoint requiere permiso, validamos el claim "perm".
            // Reglas:
            // - isSuperUser=true => acceso total (cliente/operación)
            // - claim repetible perm: "MODULE.PERMISSION_CODE" (o compat: "module.permission")
            if (!string.IsNullOrWhiteSpace(requiredPermission))
            {
                var isSuperUser = string.Equals(
                    context.User.FindFirst("isSuperUser")?.Value,
                    "true",
                    StringComparison.OrdinalIgnoreCase);

                if (!isSuperUser)
                {
                    static string Norm(string s) => s.Trim().ToLowerInvariant();

                    var requiredNorm = Norm(requiredPermission);
                    var has = context.User
                        .FindAll("perm")
                        .Select(c => c.Value)
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .Select(Norm)
                        .Any(v => v == requiredNorm);

                    if (!has)
                    {
                        await HandleBlockedAccessAsync(context, AccessResult.Forbidden);
                        return;
                    }
                }
            }

            await HandleBlockedAccessAsync(context, accessStatus);
        }

        // --- MÉTODOS AUXILIARES LIVIANOS ---

        private static Task HandleBlockedAccessAsync(HttpContext context, AccessResult status)
        {
            context.Response.ContentType = "application/json";

            var (httpCode, message) = status switch
            {
                AccessResult.CompanyDisabled => (HttpStatusCode.Unauthorized, "La cuenta de la empresa está desactivada."),
                AccessResult.SubscriptionExpired => (HttpStatusCode.PaymentRequired, "Su suscripción ha vencido. Por favor realice el pago."),
                AccessResult.ModuleNotIncluded => (HttpStatusCode.Forbidden, "Su plan actual no incluye este módulo."),
                AccessResult.Forbidden => (HttpStatusCode.Forbidden, "No tiene permisos para ejecutar esta acción."),
                _ => (HttpStatusCode.Forbidden, "Acceso denegado por políticas de suscripción.")
            };

            context.Response.StatusCode = (int)httpCode;

            // Usamos tu misma estructura de respuesta
            var response = ApiResponse<object>.Failure(message, (int)httpCode);

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
