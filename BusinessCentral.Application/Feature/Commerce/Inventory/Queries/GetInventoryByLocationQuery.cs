using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Inventory.Queries;

public sealed record GetInventoryByLocationQuery(int CompanyId, DateTime AsOfUtc, long? LocationId = null)
    : IRequest<Result<List<InventoryByLocationRowDTO>>>;

