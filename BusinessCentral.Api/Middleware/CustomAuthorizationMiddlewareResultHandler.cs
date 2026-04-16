using System.Text.Json;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;

namespace BusinessCentral.Api.Security
{
    public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Succeeded)
            {
                await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
                return;
            }

            // Intentamos leer el motivo y status code publicado por el handler
            var message = context.Items.ContainsKey("AuthFailureMessage") ? context.Items["AuthFailureMessage"]?.ToString() : null;
            var statusObj = context.Items.ContainsKey("AuthFailureStatusCode") ? context.Items["AuthFailureStatusCode"] : null;
            int statusCode = 0;
            if (statusObj != null && int.TryParse(statusObj.ToString(), out var sc)) statusCode = sc;

            // Si no hay detalle, deducimos 401 para "Challenge" o 403 para "Forbid"
            if (string.IsNullOrEmpty(message))
            {
                if (authorizeResult.Challenged)
                {
                    message = "No autenticado";
                    statusCode = statusCode == 0 ? 401 : statusCode;
                }
                else
                {
                    message = "Acceso denegado";
                    statusCode = statusCode == 0 ? 403 : statusCode;
                }
            }

            // Construir respuesta estándar ApiResponse
            context.Response.StatusCode = statusCode == 0 ? 403 : statusCode;
            context.Response.ContentType = "application/json";

            var apiResponse = BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Failure(message, context.Response.StatusCode);
            var json = JsonSerializer.Serialize(apiResponse);
            await context.Response.WriteAsync(json);
        }
    }
}