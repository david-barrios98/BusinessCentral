using Microsoft.OpenApi;

namespace BusinessCentral.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "BusinessCentral API",
                Version = "v1"
            });

            // Evita colisiones cuando varios controladores tienen clases anidadas con el mismo nombre (p. ej. AddLineRequest).
            options.CustomSchemaIds(type =>
            {
                var id = type.FullName ?? type.Name;
                return id.Replace('+', '.');
            });

            options.SchemaFilter<Filters.SwaggerSchemaFilter>();

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "BusinessCentral API v1");
            c.RoutePrefix = "swagger";
        });

        return app;
    }
}