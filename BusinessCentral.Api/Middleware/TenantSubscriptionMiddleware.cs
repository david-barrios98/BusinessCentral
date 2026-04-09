using BusinessCentral.Application.Constants;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using System.Net;
using System.Text.Json;

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
            // 1. Intentar obtener el ID desde el Token o Header (Para peticiones autenticadas)
            var companyIdStr = context.User.FindFirst("companyId")?.Value
                               ?? context.Request.Headers["companyId"].ToString();

            // 2. CASO ESPECIAL: Si es LOGIN, intentamos leer el ID del Body
            // Solo lo hacemos si el ID sigue vacío y la ruta es la de auth/login
            if (string.IsNullOrEmpty(companyIdStr) && IsLoginRoute(context))
            {
                companyIdStr = await GetCompanyIdFromLoginBody(context);
            }

            // Si después de intentar todo sigue vacío, es una ruta pública sin tenant (ej: HealthCheck)
            if (string.IsNullOrEmpty(companyIdStr))
            {
                await _next(context);
                return;
            }

            if (int.TryParse(companyIdStr, out int companyId))
            {
                var endpoint = context.GetEndpoint();
                var requiredModule = endpoint?.Metadata.GetMetadata<RequiresModuleAttribute>()?.ModuleName;

                // VALIDACIÓN CRÍTICA: Aquí es donde el SP verifica si la empresa está activa/paga
                var accessStatus = await subscriptionService.CheckAccessAsync(companyId, requiredModule);

                if (accessStatus == AccessResult.Success)
                {
                    await _next(context);
                    return;
                }

                await HandleBlockedAccessAsync(context, accessStatus);
            }
            else
            {
                await _next(context); // O manejar error de formato
            }
        }

        // --- MÉTODOS AUXILIARES LIVIANOS ---

        private bool IsLoginRoute(HttpContext context)
            => context.Request.Path.Value?.Contains("/auth/login", StringComparison.OrdinalIgnoreCase) ?? false;

        private async Task<string?> GetCompanyIdFromLoginBody(HttpContext context)
        {
            context.Request.EnableBuffering();
            // Usamos leaveOpen: true para no cerrar el stream del body
            using var reader = new StreamReader(context.Request.Body, System.Text.Encoding.UTF8, false, 1024, true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0; // REINICIAR el stream para el Controller

            if (string.IsNullOrEmpty(body)) return null;

            try
            {
                using var jsonDoc = JsonDocument.Parse(body);
                // Buscamos específicamente en tu estructura: userLogin -> companyId
                if (jsonDoc.RootElement.TryGetProperty("userLogin", out var userLogin) &&
                    userLogin.TryGetProperty("companyId", out var companyProp))
                {
                    return companyProp.ValueKind == JsonValueKind.String
                           ? companyProp.GetString()
                           : companyProp.GetRawText();
                }
            }
            catch { /* Ignorar errores de parseo aquí */ }

            return null;
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
