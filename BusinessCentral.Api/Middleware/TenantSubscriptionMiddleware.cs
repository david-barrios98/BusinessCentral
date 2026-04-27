using BusinessCentral.Application.Constants;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using System.Net;
using BusinessCentral.Api.Services;

namespace BusinessCentral.Api.Middleware
{
    public class TenantSubscriptionMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantSubscriptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ISubscriptionRepository subscriptionService, ITenantContext tenantContext)
        {
            // Si no hay tenant, es ruta pública (ej: health) o petición sin contexto.
            if (tenantContext.CompanyId is null)
            {
                await _next(context);
                return;
            }

            var endpoint = context.GetEndpoint();
            var requiredModule = endpoint?.Metadata.GetMetadata<RequiresModuleAttribute>()?.ModuleName;

            // VALIDACIÓN CRÍTICA: Aquí es donde el SP verifica si la empresa está activa/paga
            var accessStatus = await subscriptionService.CheckAccessAsync(tenantContext.CompanyId.Value, requiredModule);

            if (accessStatus == AccessResult.Success)
            {
                await _next(context);
                return;
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
                _ => (HttpStatusCode.Forbidden, "Acceso denegado por políticas de suscripción.")
            };

            context.Response.StatusCode = (int)httpCode;

            // Usamos tu misma estructura de respuesta
            var response = ApiResponse<object>.Failure(message, (int)httpCode);

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
