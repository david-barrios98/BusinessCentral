using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using BusinessCentral.Infrastructure.Persistence.Repositories;
using BusinessCentral.Shared.Helper;
using Microsoft.AspNetCore.Http;

namespace BusinessCentral.Infrastructure.Security
{
    public class SystemRoleHandler : AuthorizationHandler<SystemRoleRequirement>
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<SystemRoleHandler> _logger;
        private readonly UsersRepository _usersRepository;

        public SystemRoleHandler(
            UsersRepository usersRepository,
            IMemoryCache cache,
            ILogger<SystemRoleHandler> logger)
        {
            _usersRepository = usersRepository;
            _cache = cache;
            _logger = logger;
        }

        private void PublishFailureToHttpContext(AuthorizationHandlerContext context, string message, int statusCode)
        {
            try
            {
                var resource = context.Resource;
                if (resource == null) return;

                var prop = resource.GetType().GetProperty("HttpContext", BindingFlags.Public | BindingFlags.Instance);
                if (prop == null) return;

                var httpContext = prop.GetValue(resource) as HttpContext;
                if (httpContext == null) return;

                // Solo publicamos si realmente hay un mensaje de negocio
                httpContext.Items["AuthFailureMessage"] = message;
                httpContext.Items["AuthFailureStatusCode"] = statusCode;
            }
            catch { /* Ignorar fallos de reflexión */ }
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SystemRoleRequirement requirement)
        {
            try
            {
                // 1. Si no está autenticado, NO publicamos mensaje.
                // Dejamos que el CustomAuthorizationMiddlewareResultHandler detecte si es Expiración o Falta de Token.
                if (context.User?.Identity?.IsAuthenticated != true)
                {
                    context.Fail();
                    return;
                }

                var sub = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? context.User.FindFirst("userId")?.Value;

                if (sub == null || !int.TryParse(sub, out var userId))
                {
                    _logger.LogWarning("Authorization failed: missing or invalid user id claim.");
                    PublishFailureToHttpContext(context, "Identidad de usuario no válida", 401);
                    context.Fail();
                    return;
                }

                var cacheKey = $"systemrole_{userId}";

                if (!_cache.TryGetValue(cacheKey, out bool isSystemRole))
                {
                    var user = await _usersRepository.RolUsersAsync(userId);

                    // 2. CASOS DE NEGOCIO: Aquí sí publicamos mensajes específicos.
                    if (user.Count == 0 || !user.Exists(x => x.IsSystemRole == 1))
                    {
                        _logger.LogWarning("Authorization failed: user {UserId} not found.", userId);
                        PublishFailureToHttpContext(context, "El usuario no existe en el sistema", 404);
                        context.Fail();
                        return;
                    }

                    if (!user.Exists(x => x.UserActive))
                    {
                        _logger.LogWarning("Authorization failed: user {UserId} inactive.", userId);
                        PublishFailureToHttpContext(context, "Su cuenta de usuario está desactivada", 403);
                        context.Fail();
                        return;
                    }

                    if (!user.Exists(x => x.RolActive))
                    {
                        _logger.LogWarning("Authorization failed: user {UserId} role inactive.", userId);
                        var inactiveRole = user.FirstOrDefault(x => !x.RolActive)?.RoleName ?? "desconocido";
                        PublishFailureToHttpContext(context, $"Su rol de {inactiveRole} está desactivado", 403);
                        context.Fail();
                        return;
                    }

                    isSystemRole = user.Exists(x => x.IsSystemRole == 1);
                    _cache.Set(cacheKey, isSystemRole, TimeSpan.FromSeconds(30));
                }

                if (isSystemRole)
                {
                    context.Succeed(requirement);
                    return;
                }

                _logger.LogWarning("Authorization failed: user {UserId} role is not system role.", userId);
                PublishFailureToHttpContext(context, "No tiene permisos de administrador del sistema", 403);
                context.Fail();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authorization error in SystemRoleHandler.");
                PublishFailureToHttpContext(context, "Error interno al verificar permisos", 500);
                context.Fail();
            }
        }
    }
}