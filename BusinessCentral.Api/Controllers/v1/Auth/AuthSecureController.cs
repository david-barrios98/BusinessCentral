using BusinessCentral.Api.Common;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Feature.Auth.Commands.Logout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Auth;

[Authorize]
[Route("api/v1/secure/auth")]
public sealed class AuthSecureController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public AuthSecureController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDTO body)
    {
        var token = Request.Headers.Authorization.ToString();
        if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            token = token["Bearer ".Length..].Trim();

        var cmd = new LogoutCommand(
            UserId: body.UserId,
            CompanyId: body.CompanyId,
            RefreshToken: body.RefreshToken,
            SessionId: body.SessionId,
            Token: string.IsNullOrWhiteSpace(token) ? null : token
        );

        var result = await _mediator.Send(cmd);
        return ProcessResult(result);
    }
}

