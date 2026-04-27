using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Feature.Services.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Services;

[Authorize]
[RequiresModule("SERVICES")]
[Route("api/v1/secure/services/orders")]
public sealed class ServiceOrdersController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public ServiceOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public sealed class CreateOrderRequest
    {
        public string? VehicleType { get; set; }
        public string? Plate { get; set; }
        public string? CustomerName { get; set; }
        public string? FulfillmentMethodCode { get; set; }
        public string? FulfillmentDetails { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest req)
    {
        var result = await _mediator.Send(new CreateServiceOrderCommand(
            CompanyId,
            req.VehicleType,
            req.Plate,
            req.CustomerName,
            req.FulfillmentMethodCode,
            req.FulfillmentDetails
        ));
        return ProcessResult(result);
    }

    [HttpGet("{orderId:long}")]
    public async Task<IActionResult> Get([FromRoute] long orderId)
    {
        var result = await _mediator.Send(new GetServiceOrderQuery(CompanyId, orderId));
        return ProcessResult(result);
    }

    public sealed class AddLineRequest
    {
        public int ServiceId { get; set; }
        public decimal Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public int? EmployeeUserId { get; set; }
    }

    [HttpPost("{orderId:long}/lines")]
    public async Task<IActionResult> AddLine([FromRoute] long orderId, [FromBody] AddLineRequest req)
    {
        var result = await _mediator.Send(new AddServiceOrderLineCommand(
            CompanyId,
            orderId,
            req.ServiceId,
            req.Quantity,
            req.UnitPrice,
            req.EmployeeUserId
        ));
        return ProcessResult(result);
    }
}

