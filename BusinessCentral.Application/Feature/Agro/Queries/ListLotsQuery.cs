using BusinessCentral.Application.DTOs.Agro;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Agro.Queries;

public sealed record ListLotsQuery(int CompanyId, string? Kind = null, bool OnlyOpen = false)
    : IRequest<Result<List<AgroLotDTO>>>;

