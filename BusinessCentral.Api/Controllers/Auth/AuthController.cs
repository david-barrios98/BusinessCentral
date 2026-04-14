using BusinessCentral.Api.Common;
using BusinessCentral.Api.Controllers.Auth;
using BusinessCentral.Application.Features.Auth.Commands.Login;
using BusinessCentral.Application.Features.Auth.Commands.Logout;
using BusinessCentral.Application.Features.Auth.Commands.Refresh;
using BusinessCentral.Infrastructure.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

        /// <summary>
        /// Logout flexible: revoca por refreshToken, sessionId, userId o companyId.
        /// Se puede enviar cualquiera de los campos en el body.
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutCommand command)
        {
            var result = await _mediator.Send(command);
            return ProcessResult(result);
        }

        /// <summary>
        /// Forzar cierre de todas las sesiones y revocar refresh tokens de un usuario.
        /// </summary>
        [HttpPost("logout/user/{userId:int}")]
        public async Task<IActionResult> LogoutUser([FromRoute] int userId)
        {
            var cmd = new LogoutCommand(UserId: userId);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        /// <summary>
        /// Forzar cierre de todas las sesiones y revocar refresh tokens de una compañía.
        /// </summary>
        [HttpPost("logout/company/{companyId:int}")]
        public async Task<IActionResult> LogoutCompany([FromRoute] int companyId)
        {
            var cmd = new LogoutCommand(CompanyId: companyId);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }
    }
}