using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using BusinessCentral.Shared.Helper;

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

        int statusCode = (int)HttpStatusCode.Forbidden;
        string message = "Acceso denegado.";
        var _tokenService = context.RequestServices.GetRequiredService<ITokenService>();
        var token = string.Empty;
        var authHeader = string.Empty;

        if (authorizeResult.Succeeded)
        {
            authHeader = context.Request.Headers["Authorization"].ToString();
            token = authHeader.Substring("Bearer ".Length).Trim();

            if (await _tokenService.IsTokenRevoked(token))
            {
                message = "Token ha sido revocado";
                context.Response.Headers.Append("Token-Expired", "true");
                context.Response.Headers.Append("Access-Control-Expose-Headers", "Token-Expired");
                await WriteResponseAsync(context, statusCode, message);
                return;
            }
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
            return;
        }


        if (authorizeResult.Challenged)
        {
            authHeader = context.Request.Headers["Authorization"].ToString();
            statusCode = (int)HttpStatusCode.Unauthorized;

            if (string.IsNullOrEmpty(authHeader))
            {
                await WriteResponseAsync(context, statusCode, message);
                return;
            }
            token = authHeader.Substring("Bearer ".Length).Trim();

            if (await _tokenService.IsTokenExpired(token) || string.IsNullOrEmpty(token))
            {
                message = "La sesión ha expirado.";
                context.Response.Headers.Append("Token-Expired", "true");
                context.Response.Headers.Append("Access-Control-Expose-Headers", "Token-Expired");
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