using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Reports.Queries;

public sealed class GetFinancialSummaryHandler : IRequestHandler<GetFinancialSummaryQuery, Result<List<FinancialSummaryRowDTO>>>
{
    private readonly IFinanceReportsRepository _repo;

    public GetFinancialSummaryHandler(IFinanceReportsRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<FinancialSummaryRowDTO>>> Handle(GetFinancialSummaryQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<List<FinancialSummaryRowDTO>>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        if (request.ToDateUtc < request.FromDateUtc)
            return Result<List<FinancialSummaryRowDTO>>.Failure("Rango de fechas inválido.", "VALIDATION", "Validation");

        var data = await _repo.GetFinancialSummaryAsync(request.CompanyId, request.FromDateUtc, request.ToDateUtc);
        return Result<List<FinancialSummaryRowDTO>>.Success(data);
    }
}

