using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Puc.Queries;

public sealed record ListAccountsQuery(int CompanyId, bool OnlyActive = true, string? Q = null)
    : IRequest<Result<List<AccountDTO>>>;

