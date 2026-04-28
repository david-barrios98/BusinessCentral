using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessCentral.Api.Common;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Feature.Auth.Commands.Logout;
using BusinessCentral.Application.Feature.Auth.Commands.Refresh;
using BusinessCentral.Application.Feature.Auth.Commands.Users;

namespace BusinessCentral.Api.Controllers.v1.Users
{
    [Route("api/v1/private/users")]
    [Authorize(Policy = "SystemRole")]
    public class UsersController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator) => _mediator = mediator;


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequestDTO command)
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var cmd = new RefreshTokenCommand(command.RefreshToken, token);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        /// <summary>
        /// Logout flexible: revoca por refreshToken, sessionId, userId o companyId.
        /// Se puede enviar cualquiera de los campos en el body.
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutRequestDTO command)
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var cmd = new LogoutCommand(
                UserId: command.UserId,
                CompanyId: command.CompanyId,
                RefreshToken: command.RefreshToken,
                SessionId: command.SessionId,
                Token: token
            );
            var result = await _mediator.Send(cmd);
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO dto)
        {
            var cmd = new CreateUserCommand(dto.CompanyId, dto);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpGet("company/{companyId:int}/users/{userId:int}")]
        public async Task<IActionResult> Get([FromRoute] int companyId, [FromRoute] int userId)
        {
            var cmd = new GetUserQuery(companyId, userId);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpPut("company/{companyId:int}/users/{userId:int}")]
        public async Task<IActionResult> Update([FromRoute] int companyId, int userId, [FromBody] UpdateUserDTO dto)
        {
            dto.UserId = userId;
            var cmd = new UpdateUserCommand(companyId, dto);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpDelete("company/{companyId:int}/users/{userId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int companyId, int userId)
        {
            var cmd = new DeleteUserCommand(companyId, userId);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpGet("company/{companyId:int}")]
        public async Task<IActionResult> List([FromRoute] int companyId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var cmd = new ListUsersQuery(companyId, page, pageSize);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }
    }
}