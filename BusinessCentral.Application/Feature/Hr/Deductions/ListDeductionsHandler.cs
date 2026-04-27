using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.Deductions;

public sealed class ListDeductionsHandler : IRequestHandler<ListDeductionsQuery, Result<List<DeductionDTO>>>
{
    private readonly IHrRepository _hr;

    public ListDeductionsHandler(IHrRepository hr)
    {
        _hr = hr;
    }

    public async Task<Result<List<DeductionDTO>>> Handle(ListDeductionsQuery request, CancellationToken cancellationToken)
    {
        var list = await _hr.ListDeductionsAsync(request.CompanyId, request.UserId, request.Type);
        return Result<List<DeductionDTO>>.Success(list);
    }
}

