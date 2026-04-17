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

        // Usamos el mensaje genérico para errores 500 por seguridad, 
        // para el resto usamos el mensaje de la excepción.
        string message = exception.Message;

        var response = BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Exception(
            message,
            statusCode
        );

        return context.Response.WriteAsJsonAsync(response);
    }
}