using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Reports.Queries;

public sealed class GetPnLHandler : IRequestHandler<GetPnLQuery, Result<List<PnLRowDTO>>>
{
    private readonly IFinanceReportsRepository _repo;

    public GetPnLHandler(IFinanceReportsRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<PnLRowDTO>>> Handle(GetPnLQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<List<PnLRowDTO>>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        if (request.ToDateUtc < request.FromDateUtc)
            return Result<List<PnLRowDTO>>.Failure("Rango de fechas inválido.", "VALIDATION", "Validation");

        var data = await _repo.GetPnLAsync(request.CompanyId, request.FromDateUtc, request.ToDateUtc);
        return Result<List<PnLRowDTO>>.Success(data);
    }
}

