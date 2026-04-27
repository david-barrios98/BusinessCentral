using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Reports.Queries;

public sealed record GetRentaAnnualCoQuery(int CompanyId, int Year)
    : IRequest<Result<RentaAnnualSummaryDTO>>;

