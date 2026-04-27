using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Puc.Reports.Queries;

public sealed class GetTrialBalanceHandler : IRequestHandler<GetTrialBalanceQuery, Result<List<TrialBalanceRowDTO>>>
{
    private readonly IPucAccountingRepository _repo;

    public GetTrialBalanceHandler(IPucAccountingRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<TrialBalanceRowDTO>>> Handle(GetTrialBalanceQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<List<TrialBalanceRowDTO>>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        if (request.ToDateUtc < request.FromDateUtc)
            return Result<List<TrialBalanceRowDTO>>.Failure("Rango de fechas inválido.", "VALIDATION", "Validation");

        var data = await _repo.GetTrialBalanceAsync(request.CompanyId, request.FromDateUtc, request.ToDateUtc);
        return Result<List<TrialBalanceRowDTO>>.Success(data);
    }
}

