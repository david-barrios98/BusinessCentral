using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Reports.Queries;

public sealed record GetPnLQuery(int CompanyId, DateTime FromDateUtc, DateTime ToDateUtc)
    : IRequest<Result<List<PnLRowDTO>>>;

