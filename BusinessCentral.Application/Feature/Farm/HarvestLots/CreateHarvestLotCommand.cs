using BusinessCentral.Application.DTOs.Farm;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Farm.HarvestLots;

public sealed record CreateHarvestLotCommand(int CompanyId, HarvestLotDTO Lot) : IRequest<Result<long>>;

