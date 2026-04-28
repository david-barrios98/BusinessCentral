using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Transactions.Queries;

public sealed record ListFinancialTransactionsQuery(
    int CompanyId,
    DateTime? FromDateUtc,
    DateTime? ToDateUtc,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<FinancialTransactionListItemDTO>>>;
