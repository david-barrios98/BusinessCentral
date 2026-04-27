using BusinessCentral.Api.Common;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace BusinessCentral.Api.Controllers.v1.System;

[Authorize(Policy = "SystemRole")]
[Route("api/v1/system/auth/users")]
public sealed class UserAccessController : ApiControllerBase
{
    private readonly IUserRepository _users;
    private readonly IPublicAccessRepository _publicAccess;

    public UserAccessController(IUserRepository users, IPublicAccessRepository publicAccess)
    {
        _users = users;
        _publicAccess = publicAccess;
    }

    public sealed class SetLoginRequest
    {
        public bool CanLogin { get; set; } = true;
    }

    // Nota: IUserRepository actual no expone update parcial; este endpoint queda listo cuando agreguemos sp_update_user CanLogin.
    // Por ahora solo generamos token público (caso "sin acceso al sistema").

    [HttpPost("{companyId:int}/{userId:int}/public-tokens/hr-account")]
    public async Task<IActionResult> CreateHrAccountToken(
        [FromRoute] int companyId,
        [FromRoute] int userId,
        [FromQuery] int daysValid = 180)
    {
        // token aleatorio (no guardamos en DB; guardamos hash)
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToBase64String(tokenBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');

        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
        var tokenHashHex = Convert.ToHexString(hash).ToLowerInvariant();

        var expiresAt = DateTime.UtcNow.AddDays(Math.Clamp(daysValid, 1, 3650));

        await _publicAccess.CreatePublicTokenAsync(companyId, userId, tokenHashHex, "HR_ACCOUNT", expiresAt);

        // Se devuelve el token SOLO una vez al admin/propietario.
        return Ok(ApiResponse<object>.Success(new
        {
            companyId,
            userId,
            scope = "HR_ACCOUNT",
            token,
            expiresAtUtc = expiresAt
        }, "OK", 200));
    }
}

