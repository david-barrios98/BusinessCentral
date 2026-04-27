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

        // HS256 exige clave simétrica ≥ 256 bits (32 bytes). Ver IDX10720 si es más corta.
        var keyByteCount = Encoding.UTF8.GetByteCount(secretKey);
        if (keyByteCount < 32)
        {
            throw new InvalidOperationException(
                $"JwtSettings:SecretKey debe tener al menos 32 bytes en UTF-8 (actual: {keyByteCount}). " +
                "Amplía la cadena o usa una clave aleatoria de 32+ caracteres ASCII para desarrollo.");
        }

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