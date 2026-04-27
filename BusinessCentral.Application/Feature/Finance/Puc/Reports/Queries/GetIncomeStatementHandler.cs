using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Puc.Reports.Queries;

public sealed class GetIncomeStatementHandler : IRequestHandler<GetIncomeStatementQuery, Result<List<PucClassSummaryRowDTO>>>
{
    private readonly IPucAccountingRepository _repo;

    public GetIncomeStatementHandler(IPucAccountingRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<PucClassSummaryRowDTO>>> Handle(GetIncomeStatementQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<List<PucClassSummaryRowDTO>>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        if (request.ToDateUtc < request.FromDateUtc)
            return Result<List<PucClassSummaryRowDTO>>.Failure("Rango de fechas inválido.", "VALIDATION", "Validation");

        var data = await _repo.GetIncomeStatementAsync(request.CompanyId, request.FromDateUtc, request.ToDateUtc);
        return Result<List<PucClassSummaryRowDTO>>.Success(data);
    }
}

