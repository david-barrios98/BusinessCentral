using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.Loans;

public sealed class ListLoanAdvancesHandler : IRequestHandler<ListLoanAdvancesQuery, Result<List<LoanAdvanceDTO>>>
{
    private readonly IHrRepository _hr;

    public ListLoanAdvancesHandler(IHrRepository hr)
    {
        _hr = hr;
    }

    public async Task<Result<List<LoanAdvanceDTO>>> Handle(ListLoanAdvancesQuery request, CancellationToken cancellationToken)
    {
        var list = await _hr.ListLoanAdvancesAsync(request.CompanyId, request.UserId, request.Status);
        return Result<List<LoanAdvanceDTO>>.Success(list);
    }
}

