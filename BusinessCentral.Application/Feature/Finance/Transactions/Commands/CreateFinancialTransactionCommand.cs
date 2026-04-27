using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Transactions.Commands;

public sealed record CreateFinancialTransactionCommand(int CompanyId, CreateFinancialTransactionDTO Dto)
    : IRequest<Result<long>>;

