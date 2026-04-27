using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Application.Ports.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Config;

[Authorize]
[Route("api/v1/secure/config/payment-methods")]
public sealed class PaymentMethodsController : SecureCompanyControllerBase
{
    private readonly IPaymentMethodRepository _repo;

    public PaymentMethodsController(IPaymentMethodRepository repo)
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

