using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessCentral.Api.Common;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Features.Auth.Commands.Users;

namespace BusinessCentral.Api.Controllers
{
    [Route("api/users")]
    [Authorize(Policy = "SystemRole")]
    public class UsersController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO dto)
        {
            var cmd = new CreateUserCommand(dto);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpGet("{userId:int}")]
        public async Task<IActionResult> Get(int userId)
        {
            var cmd = new GetUserQuery(userId);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpPut("{userId:int}")]
        public async Task<IActionResult> Update(int userId, [FromBody] UpdateUserDTO dto)
        {
            dto.UserId = userId;
            var cmd = new UpdateUserCommand(dto);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpDelete("{userId:int}")]
        public async Task<IActionResult> Delete(int userId)
        {
            var cmd = new DeleteUserCommand(userId);
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