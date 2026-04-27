using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Transactions.Commands;

public sealed class CreateFinancialTransactionHandler : IRequestHandler<CreateFinancialTransactionCommand, Result<long>>
{
    private readonly IFinanceReportsRepository _repo;

    public CreateFinancialTransactionHandler(IFinanceReportsRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<long>> Handle(CreateFinancialTransactionCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<long>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        var id = await _repo.CreateFinancialTransactionAsync(request.CompanyId, request.Dto);
        return Result<long>.Success(id);
    }
}

