using BusinessCentral.Application.DTOs.Farm;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Farm.HarvestLots;

public sealed record ListHarvestLotsQuery(
    int CompanyId,
    DateTime? FromDate,
    DateTime? ToDate,
    int? ZoneId
) : IRequest<Result<List<HarvestLotDTO>>>;

