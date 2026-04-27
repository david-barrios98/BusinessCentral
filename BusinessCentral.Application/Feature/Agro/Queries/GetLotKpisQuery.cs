using BusinessCentral.Application.DTOs.Agro;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Agro.Queries;

public sealed record GetLotKpisQuery(int CompanyId, long LotId)
    : IRequest<Result<AgroLotKpisDTO>>;

