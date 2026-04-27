using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Reports.Queries;

public sealed class GetTaxSummaryCoHandler : IRequestHandler<GetTaxSummaryCoQuery, Result<List<TaxSummaryRowDTO>>>
{
    private readonly IFinanceReportsRepository _repo;

    public GetTaxSummaryCoHandler(IFinanceReportsRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<TaxSummaryRowDTO>>> Handle(GetTaxSummaryCoQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<List<TaxSummaryRowDTO>>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        if (request.ToDateUtc < request.FromDateUtc)
            return Result<List<TaxSummaryRowDTO>>.Failure("Rango de fechas inválido.", "VALIDATION", "Validation");

        var data = await _repo.GetTaxSummaryCoAsync(request.CompanyId, request.FromDateUtc, request.ToDateUtc);
        return Result<List<TaxSummaryRowDTO>>.Success(data);
    }
}

