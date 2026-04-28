using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Transactions.Queries;

public sealed class ListFinancialTransactionsHandler
    : IRequestHandler<ListFinancialTransactionsQuery, Result<PagedResult<FinancialTransactionListItemDTO>>>
{
    private readonly IFinanceReportsRepository _repo;

    public ListFinancialTransactionsHandler(IFinanceReportsRepository repo) => _repo = repo;

    public async Task<Result<PagedResult<FinancialTransactionListItemDTO>>> Handle(
        ListFinancialTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var p = await _repo.ListFinancialTransactionsAsync(
            request.CompanyId,
            request.FromDateUtc,
            request.ToDateUtc,
            request.Page,
            request.PageSize);
        return Result<PagedResult<FinancialTransactionListItemDTO>>.Success(p);
    }
}
