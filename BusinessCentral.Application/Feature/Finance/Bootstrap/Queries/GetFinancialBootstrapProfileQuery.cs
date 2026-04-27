using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Bootstrap.Queries;

public sealed record GetFinancialBootstrapProfileQuery(int CompanyId)
    : IRequest<Result<CompanyFinancialBootstrapDTO>>;
