using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using BusinessCentral.Infrastructure.Persistence.Adapters;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

namespace BusinessCentral.Infrastructure.Security
{
    public class SystemRoleHandler : AuthorizationHandler<SystemRoleRequirement>
    {
        private readonly BusinessCentralDbContext _db;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SystemRoleHandler> _logger;

        public SystemRoleHandler(
            BusinessCentralDbContext db,
            IMemoryCache cache,
            ILogger<SystemRoleHandler> logger)
        {
            _db = db;
            _cache = cache;
            _logger = logger;
        }

        private void PublishFailureToHttpContext(AuthorizationHandlerContext context, string message, int statusCode)
        {
            try
            {
                // context.Resource normalmente es AuthorizationFilterContext en MVC,
                // pero para evitar referencia directa usamos reflexión para extraer HttpContext si existe.
                var resource = context.Resource;
                if (resource == null) return;

                var prop = resource.GetType().GetProperty("HttpContext", BindingFlags.Public | BindingFlags.Instance);
                if (prop == null) return;

                var httpContext = prop.GetValue(resource) as HttpContext;
                if (httpContext == null) return;

                httpContext.Items["AuthFailureMessage"] = message;
                httpContext.Items["AuthFailureStatusCode"] = statusCode;
            }
            catch
            {
                // No hacemos nada si falla la reflexión
            }
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SystemRoleRequirement requirement)
        {
            try
            {
                if (context.User?.Identity?.IsAuthenticated != true)
                {
                    _logger.LogWarning("Authorization failed: principal not authenticated.");
                    PublishFailureToHttpContext(context, "No autenticado", 401);
                    context.Fail();
                    return;
                }

                var sub = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? context.User.FindFirst("userId")?.Value;
                if (!int.TryParse(sub, out var userId))
                {
                    _logger.LogWarning("Authorization failed: missing or invalid user id claim.");
                    PublishFailureToHttpContext(context, "Usuario inválido", 401);
                    context.Fail();
                    return;
                }

                var cacheKey = $"systemrole_{userId}";
                if (!_cache.TryGetValue(cacheKey, out bool isSystemRole))
                {
                    var user = await _db.Users
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.Id == userId);

                    if (user == null)
                    {
                        _logger.LogWarning("Authorization failed: user {UserId} not found.", userId);
                        PublishFailureToHttpContext(context, "Usuario no encontrado", 404);
                        context.Fail();
                        return;
                    }

                    if (!user.Active)
                    {
                        _logger.LogWarning("Authorization failed: user {UserId} inactive.", userId);
                        PublishFailureToHttpContext(context, "Usuario inactivo", 403);
                        context.Fail();
                        return;
                    }

                    isSystemRole = user.Role != null && user.Role.IsSystemRole;
                    _cache.Set(cacheKey, isSystemRole, TimeSpan.FromSeconds(30));
                }

                if (isSystemRole)
                {
                    context.Succeed(requirement);
                    return;
                }

                _logger.LogWarning("Authorization failed: user {UserId} role is not system role.", userId);
                PublishFailureToHttpContext(context, "No autorizado: rol no autorizado", 403);
                context.Fail();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authorization error in SystemRoleHandler.");
                PublishFailureToHttpContext(context, "Error interno de autorización", 500);
                context.Fail();
            }
        }
    }
}