using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Puc.Queries;

public sealed class ListAccountsHandler : IRequestHandler<ListAccountsQuery, Result<List<AccountDTO>>>
{
    private readonly IPucAccountingRepository _repo;

    public ListAccountsHandler(IPucAccountingRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<AccountDTO>>> Handle(ListAccountsQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<List<AccountDTO>>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        var data = await _repo.ListAccountsAsync(request.CompanyId, request.OnlyActive, request.Q);
        return Result<List<AccountDTO>>.Success(data);
    }
}

