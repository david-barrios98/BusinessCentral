using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Puc.Reports.Queries;

public sealed class GetBalanceSheetHandler : IRequestHandler<GetBalanceSheetQuery, Result<List<PucClassSummaryRowDTO>>>
{
    private readonly IPucAccountingRepository _repo;

    public GetBalanceSheetHandler(IPucAccountingRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<PucClassSummaryRowDTO>>> Handle(GetBalanceSheetQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<List<PucClassSummaryRowDTO>>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        var data = await _repo.GetBalanceSheetAsync(request.CompanyId, request.ToDateUtc);
        return Result<List<PucClassSummaryRowDTO>>.Success(data);
    }
}

