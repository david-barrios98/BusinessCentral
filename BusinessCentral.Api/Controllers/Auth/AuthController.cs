using BusinessCentral.Api.Common;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Feature.Auth.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.Auth
{
    [Route("api/public/auth")]
    public class AuthController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO requestDto)
        {
            var command = new LoginCommand(
                requestDto,
                IpAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent: Request.Headers["User-Agent"].ToString(),
                Platform: Request.Headers["sec-ch-ua-platform"].ToString().Trim('"') ?? "Undefaund" // Puedes usar un header personalizado
            );
            var result = await _mediator.Send(command);
            return ProcessResult(result);
        }

        [HttpPost("password/forgot")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] BusinessCentral.Application.DTOs.Auth.PasswordResetRequestDTO request)
        {
            var cmd = new BusinessCentral.Application.Features.Auth.Commands.PasswordReset.RequestPasswordResetCommand(request);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpPost("password/reset")]
        public async Task<IActionResult> ConfirmPasswordReset([FromBody] BusinessCentral.Application.DTOs.Auth.PasswordResetConfirmDTO request)
        {
            var cmd = new BusinessCentral.Application.Features.Auth.Commands.PasswordReset.ConfirmPasswordResetCommand(request);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }
    }
}