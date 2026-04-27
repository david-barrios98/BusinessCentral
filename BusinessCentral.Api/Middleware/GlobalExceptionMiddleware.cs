using BusinessCentral.Application.Constants;
using BusinessCentral.Application.DTOs.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace BusinessCentral.Api.Middleware;

/// <summary>
/// Middleware global para manejo de excepciones
/// Proporciona respuestas consistentes y logging detallado
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception - TraceId: {TraceId}", context.TraceIdentifier);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Determinamos el status code basado en el tipo de excepción
        int statusCode = exception switch
        {
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            System.ComponentModel.DataAnnotations.ValidationException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        // Para 500 devolvemos un mensaje genérico (no filtrar detalles internos).
        string message = statusCode >= 500
            ? "Ocurrió un error interno. Intenta nuevamente."
            : exception.Message;

        var response = BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Exception(
            message,
            statusCode,
            context.TraceIdentifier
        );

        return context.Response.WriteAsJsonAsync(response);
    }
}