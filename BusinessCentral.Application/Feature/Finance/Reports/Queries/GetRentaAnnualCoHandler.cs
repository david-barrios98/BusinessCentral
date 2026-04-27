using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Reports.Queries;

public sealed class GetRentaAnnualCoHandler : IRequestHandler<GetRentaAnnualCoQuery, Result<RentaAnnualSummaryDTO>>
{
    private readonly IFinanceReportsRepository _repo;

    public GetRentaAnnualCoHandler(IFinanceReportsRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<RentaAnnualSummaryDTO>> Handle(GetRentaAnnualCoQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<RentaAnnualSummaryDTO>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        if (request.Year < 2000 || request.Year > DateTime.UtcNow.Year + 1)
            return Result<RentaAnnualSummaryDTO>.Failure("Año inválido.", "VALIDATION", "Validation");

        var data = await _repo.GetRentaAnnualCoAsync(request.CompanyId, request.Year);
        return Result<RentaAnnualSummaryDTO>.Success(data);
    }
}

