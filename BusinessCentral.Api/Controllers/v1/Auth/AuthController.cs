using BusinessCentral.Api.Common;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Feature.Auth.Commands.Login;
using BusinessCentral.Application.Feature.Auth.Commands.PasswordReset;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Auth
{
    [Route("api/v1/public/auth")]
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
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDTO request)
        {
            var cmd = new RequestPasswordResetCommand(request);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpPost("password/reset")]
        public async Task<IActionResult> ConfirmPasswordReset([FromBody] PasswordResetConfirmDTO request)
        {
            var cmd = new ConfirmPasswordResetCommand(request);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }
    }
}