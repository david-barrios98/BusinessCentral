using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using BusinessCentral.Application.Constants;
using BusinessCentral.Application.DTOs.Common;
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
        int code = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = code;

        var response = ApiResponse<object>.Exception(
            exception.Message,
            code
        );

        return context.Response.WriteAsJsonAsync(response);
    }

    private static string GetErrorMessage(Exception exception) => exception switch
    {
        UnauthorizedAccessException => "No autorizado",
        ValidationException => "Validaci�n fallida",
        ArgumentException => "Argumento inv�lido",
        _ => "Ha ocurrido un error inesperado"
    };
}