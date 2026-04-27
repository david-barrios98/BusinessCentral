using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Feature.Manufacturing.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Manufacturing;

[Authorize]
[RequiresModule("MFG")]
[Route("api/v1/secure/mfg/production")]
public sealed class ProductionController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public ProductionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public sealed class CreateBatchRequest
    {
        public long RecipeId { get; set; }
        public int OutputProductId { get; set; }
        public long? OutputVariantId { get; set; }
        public decimal QuantityProduced { get; set; }
        public long? ToLocationId { get; set; }
        public DateTime BatchDate { get; set; }
        public string? Notes { get; set; }
    }

    [HttpPost("batches")]
    public async Task<IActionResult> CreateBatch([FromBody] CreateBatchRequest req)
    {
        var result = await _mediator.Send(new CreateProductionBatchCommand(
            CompanyId,
            req.RecipeId,
            req.OutputProductId,
            req.OutputVariantId,
            req.QuantityProduced,
            req.ToLocationId,
            req.BatchDate,
            req.Notes
        ));
        return ProcessResult(result);
    }

    [HttpPost("batches/{batchId:long}/post")]
    public async Task<IActionResult> Post([FromRoute] long batchId)
    {
        var result = await _mediator.Send(new PostProductionBatchCommand(CompanyId, batchId));
        return ProcessResult(result);
    }
}

