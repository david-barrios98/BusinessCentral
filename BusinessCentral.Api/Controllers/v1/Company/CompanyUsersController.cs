using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Feature.Auth.Commands.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Company;

[Authorize]
[RequiresModule("AUTH")]
[Route("api/v1/secure/company/users")]
public sealed class CompanyUsersController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public CompanyUsersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new ListUsersQuery(CompanyId, page, pageSize));
        return ProcessResult(result);
    }

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> Get([FromRoute] int userId)
    {
        var result = await _mediator.Send(new GetUserQuery(CompanyId, userId));
        return ProcessResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDTO dto)
    {
        var result = await _mediator.Send(new CreateUserCommand(CompanyId, dto));
        return ProcessResult(result);
    }

    [HttpPut("{userId:int}")]
    public async Task<IActionResult> Update([FromRoute] int userId, [FromBody] UpdateUserDTO dto)
    {
        dto.UserId = userId;
        var result = await _mediator.Send(new UpdateUserCommand(CompanyId, dto));
        return ProcessResult(result);
    }

    [HttpDelete("{userId:int}")]
    public async Task<IActionResult> Delete([FromRoute] int userId)
    {
        var result = await _mediator.Send(new DeleteUserCommand(CompanyId, userId));
        return ProcessResult(result);
    }
}
