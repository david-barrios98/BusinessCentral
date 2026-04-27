using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Hr.Employees;
using BusinessCentral.Application.Feature.Hr.Availability;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Hr;

[Authorize]
[RequiresModule("HR")]
[Route("api/v1/secure/hr/employees")]
public sealed class EmployeesController : HrControllerBase
{
    private readonly IMediator _mediator;

    public EmployeesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{userId:int}/profile")]
    public async Task<IActionResult> GetProfile([FromRoute] int userId)
    {
        var result = await _mediator.Send(new GetEmployeeProfileQuery(CompanyId, userId));
        return ProcessResult(result);
    }

    [HttpPut("{userId:int}/profile")]
    public async Task<IActionResult> UpsertProfile([FromRoute] int userId, [FromBody] EmployeeProfileDTO dto)
    {
        dto.UserId = userId;
        dto.CompanyId = CompanyId;
        var result = await _mediator.Send(new UpsertEmployeeProfileCommand(CompanyId, dto));
        return ProcessResult(result);
    }

    [HttpGet("{userId:int}/availability")]
    public async Task<IActionResult> GetAvailability([FromRoute] int userId)
    {
        var result = await _mediator.Send(new GetEmployeeAvailabilityQuery(CompanyId, userId));
        return ProcessResult(result);
    }

    [HttpPut("{userId:int}/availability")]
    public async Task<IActionResult> UpsertAvailability([FromRoute] int userId, [FromBody] EmployeeAvailabilityDTO dto)
    {
        dto.UserId = userId;
        dto.CompanyId = CompanyId;
        var result = await _mediator.Send(new UpsertEmployeeAvailabilityCommand(CompanyId, dto));
        return ProcessResult(result);
    }
}

