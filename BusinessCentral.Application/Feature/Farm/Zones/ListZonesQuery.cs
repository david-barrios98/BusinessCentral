using BusinessCentral.Application.DTOs.Farm;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Farm.Zones;

public sealed record ListZonesQuery(int CompanyId, bool OnlyActive) : IRequest<Result<List<FarmZoneDTO>>>;

