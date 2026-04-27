using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Feature.Agro.Commands;
using BusinessCentral.Application.Feature.Agro.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Agro;

[Authorize]
[RequiresModule("AGRO")]
[Route("api/v1/secure/agro")]
public sealed class AgroController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public AgroController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("lots")]
    public async Task<IActionResult> ListLots(
        [FromQuery] string? kind = null,
        [FromQuery] bool onlyOpen = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var result = await _mediator.Send(new ListLotsQuery(CompanyId, kind, onlyOpen, page, pageSize));
        return ProcessResult(result);
    }

    public sealed class CreateLotRequest
    {
        public string Kind { get; set; } = "POULTRY"; // POULTRY/AQUA
        public string Code { get; set; } = string.Empty;
        public string? Name { get; set; }
        public DateTime StartDate { get; set; }
        public int InitialUnits { get; set; }
        public decimal? InitialAvgWeightKg { get; set; }
        public string? Notes { get; set; }
    }

    [HttpPost("lots")]
    public async Task<IActionResult> CreateLot([FromBody] CreateLotRequest req)
    {
        var result = await _mediator.Send(new CreateLotCommand(
            CompanyId, req.Kind, req.Code, req.Name, req.StartDate, req.InitialUnits, req.InitialAvgWeightKg, req.Notes
        ));
        return ProcessResult(result);
    }

    public sealed class CreateFeedRequest
    {
        public long LotId { get; set; }
        public DateTime FeedDate { get; set; }
        public int FeedProductId { get; set; }
        public long? FeedVariantId { get; set; }
        public decimal Quantity { get; set; }
        public long? FromLocationId { get; set; }
        public decimal? UnitCost { get; set; }
        public string? Notes { get; set; }
    }

    [HttpPost("feed")]
    public async Task<IActionResult> Feed([FromBody] CreateFeedRequest req)
    {
        var result = await _mediator.Send(new CreateFeedLogCommand(
            CompanyId, req.LotId, req.FeedDate, req.FeedProductId, req.FeedVariantId, req.Quantity, req.FromLocationId, req.UnitCost, req.Notes
        ));
        return ProcessResult(result);
    }

    public sealed class CreateMortalityRequest
    {
        public long LotId { get; set; }
        public DateTime MortalityDate { get; set; }
        public int Units { get; set; }
        public decimal? AvgWeightKg { get; set; }
        public string? Notes { get; set; }
    }

    [HttpPost("mortality")]
    public async Task<IActionResult> Mortality([FromBody] CreateMortalityRequest req)
    {
        var result = await _mediator.Send(new CreateMortalityLogCommand(
            CompanyId, req.LotId, req.MortalityDate, req.Units, req.AvgWeightKg, req.Notes
        ));
        return ProcessResult(result);
    }

    public sealed class CreateHarvestRequest
    {
        public long LotId { get; set; }
        public DateTime HarvestDate { get; set; }
        public int OutputProductId { get; set; }
        public long? OutputVariantId { get; set; }
        public int Units { get; set; }
        public decimal? TotalWeightKg { get; set; }
        public long? ToLocationId { get; set; }
        public string? Notes { get; set; }
    }

    [HttpPost("harvest")]
    public async Task<IActionResult> Harvest([FromBody] CreateHarvestRequest req)
    {
        var result = await _mediator.Send(new CreateHarvestCommand(
            CompanyId, req.LotId, req.HarvestDate, req.OutputProductId, req.OutputVariantId, req.Units, req.TotalWeightKg, req.ToLocationId, req.Notes
        ));
        return ProcessResult(result);
    }

    [HttpGet("lots/{lotId:long}/kpis")]
    public async Task<IActionResult> LotKpis([FromRoute] long lotId)
    {
        var result = await _mediator.Send(new GetLotKpisQuery(CompanyId, lotId));
        return ProcessResult(result);
    }
}

