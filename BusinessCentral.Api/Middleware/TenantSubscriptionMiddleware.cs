using BusinessCentral.Application.DTOs.Common;
using System.Net;
using BusinessCentral.Application.Constants;
using BusinessCentral.Application.Ports.Outbound;

namespace BusinessCentral.Api.Middleware
{
    public class TenantSubscriptionMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantSubscriptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ISubscriptionRepository subscriptionService)
        {
            // 1. Extraer CompanyId del Token JWT (Claim)
            var companyIdClaim = context.User.FindFirst("company_id")?.Value;

            // Si no hay ID de compañía (ej: rutas de login o registro), dejamos pasar
            if (string.IsNullOrEmpty(companyIdClaim))
            {
                await _next(context);
                return;
            }

            int companyId = int.Parse(companyIdClaim);

            // 2. Identificar si el endpoint requiere un módulo específico
            var endpoint = context.GetEndpoint();
            var requiredModule = endpoint?.Metadata.GetMetadata<RequiresModuleAttribute>()?.ModuleName;

            // 3. Validar acceso (Este servicio debe usar Cache para ser rápido)
            var accessStatus = await subscriptionService.CheckAccessAsync(companyId, requiredModule);

            if (accessStatus == AccessResult.Success)
            {
                await _next(context);
                return;
            }

            // 4. Manejo de bloqueos usando tu formato ApiResponse
            await HandleBlockedAccessAsync(context, accessStatus);
        }

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
