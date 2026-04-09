using Microsoft.AspNetCore.Mvc;
using BusinessCentral.Application.DTOs.Common; // Asegúrate de que este sea el namespace de tu ApiResponse

namespace BusinessCentral.Api.Extensions
{
    public static class ValidationExtensions
    {
        public static IServiceCollection AddCustomValidationResponse(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    // Extraemos los errores y los formateamos de forma limpia
                    var errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => x.ErrorMessage)
                        .Distinct() // Evitamos mensajes duplicados
                        .ToArray();

                    var errorMessage = string.Join(" | ", errors);

                    // Usamos tu estructura estándar de respuesta
                    var response = ApiResponse<object>.Failure(errorMessage, 400);

                    return new BadRequestObjectResult(response);
                };
            });

            return services;
        }
    }
}