using BusinessCentral.Core.Application.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BusinessCentral.Infrastructure.Helpers;

namespace BusinessCentral.Shared.Helper
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public JwtService(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public string GenerateJwtToken(JwtUserDto user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            // Claims básicos + extendidos
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.userId.ToString()),
                new Claim("userId", user.userId.ToString()),
                new Claim("userName", user.userName),
                new Claim("companyId", user.companyId),
                new Claim("companyName", user.companyName),
                new Claim("loginField", user.LoginField ?? string.Empty),
                new Claim("role", user.role ?? string.Empty),
                new Claim("isSystemRole", user.isSystemRole ? "true" : "false"),
                new Claim("isSuperUser", user.isSuperUser ? "true" : "false"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Claims repetibles: permisos y módulos habilitados
            if (user.permissions != null)
            {
                foreach (var p in user.permissions.Where(p => !string.IsNullOrWhiteSpace(p)))
                    claims.Add(new Claim("perm", p));
            }

            if (user.modules != null)
            {
                foreach (var m in user.modules.Where(m => !string.IsNullOrWhiteSpace(m)))
                    claims.Add(new Claim("module", m));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: TimeZoneHelper.GetColombiaTime().AddMinutes(double.Parse(jwtSettings["ExpiryInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public int GetAccessTokenExpirationSeconds()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var expiryMinutes = double.Parse(jwtSettings["ExpiryInMinutes"] ?? "15");
            return (int)(expiryMinutes * 60);
        }

        /// <summary>
        /// Valida el token y devuelve los claims. Por defecto no valida la expiración (mantiene compatibilidad).
        /// Pasa validateLifetime = true para validar también la expiración.
        /// </summary>
        public ClaimsPrincipal? ValidateTokenAndGetClaims(string token, bool validateLifetime = false)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = validateLifetime, // <- configurable
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Intenta validar token incluyendo la expiración. Devuelve true si firma + expiración + issuer/audience son correctos.
        /// No hace comprobación de revocación en BD.
        /// </summary>
        public bool TryValidateToken(string token, out ClaimsPrincipal? principal)
        {
            principal = ValidateTokenAndGetClaims(token, validateLifetime: true);
            return principal != null;
        }

        /// <summary>
        /// Extrae el userId del token (sub). Lanza si no es válido.
        /// </summary>
        public int GetUserIdFromJwt(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                throw new SecurityTokenException("Invalid token");

            var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub);

            if (userIdClaim == null)
                throw new SecurityTokenException("User ID not found in token");

            return int.Parse(userIdClaim.Value);
        }

        /// <summary>
        /// Devuelve la fecha de expiración (convertida a Colombia) si existe.
        /// </summary>
        public DateTime? GetExpirationTime(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);

                if (expClaim != null)
                {
                    var expirationTimeUnix = long.Parse(expClaim.Value);
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expirationTimeUnix).UtcDateTime;
                    return TimeZoneHelper.ConvertToColombiaTime(expirationTime);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException("Error al leer el token JWT: " + ex.Message);
            }
        }

        /// <summary>
        /// Indica si el token está expirado según su claim exp.
        /// </summary>
        public bool IsTokenExpired(string token)
        {
            try
            {
                var exp = GetExpirationTime(token);
                if (!exp.HasValue) return false;
                return exp.Value.ToUniversalTime() <= TimeZoneHelper.GetColombiaTimeNow();
            }
            catch
            {
                // Si no se puede leer, consideramos inválido/expirado
                return true;
            }
        }
    }
}
