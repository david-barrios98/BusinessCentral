using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Puc.Reports.Queries;

public sealed record GetBalanceSheetQuery(int CompanyId, DateTime ToDateUtc)
    : IRequest<Result<List<PucClassSummaryRowDTO>>>;

