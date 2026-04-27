using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Config;

[Authorize]
[Route("api/v1/secure/config/fulfillment-methods")]
public sealed class FulfillmentMethodsController : SecureCompanyControllerBase
{
    private readonly IFulfillmentMethodRepository _repo;

    public FulfillmentMethodsController(IFulfillmentMethodRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> ListForCompany([FromQuery] bool onlyEnabled = true)
    {
        var data = await _repo.ListCompanyMethodsAsync(CompanyId, onlyEnabled);
        return Ok(BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Success(data, "OK", 200));
    }
}

