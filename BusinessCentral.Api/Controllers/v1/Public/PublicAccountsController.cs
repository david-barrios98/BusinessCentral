using BusinessCentral.Api.Common;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace BusinessCentral.Api.Controllers.v1.Public;

[Route("api/v1/public/accounts")]
public sealed class PublicAccountsController : ApiControllerBase
{
    private readonly IPublicAccessRepository _publicAccess;

    public PublicAccountsController(IPublicAccessRepository publicAccess)
    {
        _publicAccess = publicAccess;
    }

    [HttpGet("hr/summary")]
    public async Task<IActionResult> HrSummary([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return StatusCode(422, ApiResponse<object>.Failure("Token requerido.", 422));

        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
        var tokenHashHex = Convert.ToHexString(hash).ToLowerInvariant();

        var data = await _publicAccess.GetHrAccountSummaryByTokenAsync(tokenHashHex);
        return Ok(ApiResponse<object>.Success(data, "OK", 200));
    }
}

