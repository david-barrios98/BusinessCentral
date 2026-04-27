using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.DTOs.Manufacturing;
using BusinessCentral.Application.Feature.Manufacturing.Commands;
using BusinessCentral.Application.Feature.Manufacturing.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Manufacturing;

[Authorize]
[RequiresModule("MFG")]
[Route("api/v1/secure/mfg/recipes")]
public sealed class RecipesController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public RecipesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public sealed class UpsertRecipeRequest
    {
        public long? Id { get; set; }
        public int OutputProductId { get; set; }
        public long? OutputVariantId { get; set; }
        public decimal OutputQuantity { get; set; } = 1;
        public string? Notes { get; set; }
        public bool Active { get; set; } = true;
    }

    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] UpsertRecipeRequest req)
    {
        var result = await _mediator.Send(new UpsertRecipeCommand(
            CompanyId, req.Id, req.OutputProductId, req.OutputVariantId, req.OutputQuantity, req.Notes, req.Active
        ));
        return ProcessResult(result);
    }

    public sealed class SetItemsRequest
    {
        public List<RecipeItemUpsertDTO> Items { get; set; } = new();
    }

    [HttpPut("{recipeId:long}/items")]
    public async Task<IActionResult> SetItems([FromRoute] long recipeId, [FromBody] SetItemsRequest req)
    {
        var result = await _mediator.Send(new SetRecipeItemsCommand(CompanyId, recipeId, req.Items));
        return ProcessResult(result);
    }

    [HttpGet("{recipeId:long}/cost")]
    public async Task<IActionResult> Cost([FromRoute] long recipeId, [FromQuery] decimal quantity = 1)
    {
        var result = await _mediator.Send(new GetRecipeCostQuery(CompanyId, recipeId, quantity));
        return ProcessResult(result);
    }
}

