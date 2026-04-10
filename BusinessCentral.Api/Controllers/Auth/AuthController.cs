using MediatR;
using Microsoft.AspNetCore.Mvc;
using BusinessCentral.Api.Common;
using BusinessCentral.Application.Features.Auth.Commands.Login;
using BusinessCentral.Application.Features.Auth.Commands.Refresh;
using BusinessCentral.Application.Features.Auth.Commands.Logout;

namespace BusinessCentral.Api.Controllers.Auth
{
    [Route("api/auth")]
    public class AuthController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return ProcessResult(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);
            return ProcessResult(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutCommand command)
        {
            var result = await _mediator.Send(command);
            return ProcessResult(result);
        }
    }
}