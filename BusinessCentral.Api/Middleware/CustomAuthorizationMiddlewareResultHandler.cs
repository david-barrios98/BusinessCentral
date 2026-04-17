using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BusinessCentral.Application.DTOs.Common;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http.Json;

namespace BusinessCentral.Api.Middleware;

public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
    private readonly JsonOptions _jsonOptions;

    public CustomAuthorizationMiddlewareResultHandler(IOptions<JsonOptions> jsonOptions)
    {
        _jsonOptions = jsonOptions.Value;
    }

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Succeeded)
        {
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
            return;
        }

        int statusCode = (int)HttpStatusCode.Forbidden;
        string message = "Acceso denegado.";

        if (authorizeResult.Challenged)
        {
            statusCode = (int)HttpStatusCode.Unauthorized;
            message = "Usuario no autenticado.";

            // --- LÓGICA DE DETECCIÓN MANUAL DE EXPIRACIÓN ---
            // Si el Challenge falló, extraemos el token del header para ver por qué falló
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();

                if (handler.CanReadToken(token))
                {
                    var jwtToken = handler.ReadJwtToken(token);
                    // Comparamos la fecha de expiración del token con la hora actual
                    if (jwtToken.ValidTo < DateTime.UtcNow)
                    {
                        message = "La sesión ha expirado.";
                        context.Response.Headers.Append("Token-Expired", "true");
                        context.Response.Headers.Append("Access-Control-Expose-Headers", "Token-Expired");
                    }
                }
            }
        }

        // Prioridad a mensajes de lógica de negocio (ej: Usuario Inactivo)
        if (context.Items.TryGetValue("AuthFailureMessage", out var customMsg) && customMsg is string msg)
            message = msg;

        if (context.Items.TryGetValue("AuthFailureStatusCode", out var customCode) && customCode is int sc && sc != 0)
            statusCode = sc;

        await WriteResponseAsync(context, statusCode, message);
    }

    private async Task WriteResponseAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        var apiResponse = ApiResponse<object>.Failure(message, statusCode);
        await context.Response.WriteAsJsonAsync(apiResponse, _jsonOptions.SerializerOptions);
    }
}