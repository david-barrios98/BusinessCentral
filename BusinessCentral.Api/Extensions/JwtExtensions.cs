using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BusinessCentral.Api.Extensions;

public static class JwtExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var jwtSettings = config.GetSection("JwtSettings");
        var secretKey =
            jwtSettings["SecretKey"] ??
            Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ??
            Environment.GetEnvironmentVariable("JwtSettings__SecretKey");

        if (string.IsNullOrEmpty(secretKey))
            throw new InvalidOperationException("La SecretKey de JWT no está configurada.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

                    // OPCIONAL: Elimina el margen de 5 minutos de gracia
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            // Forzamos el header antes de que cualquier otra cosa escriba la respuesta
                            context.Response.Headers.Append("Token-Expired", "true");
                            context.Response.Headers.Append("Access-Control-Expose-Headers", "Token-Expired");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}