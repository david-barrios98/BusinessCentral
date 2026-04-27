using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.PaySchemes;

public sealed class ListPaySchemesHandler : IRequestHandler<ListPaySchemesQuery, Result<List<PaySchemeDTO>>>
{
    private readonly IHrRepository _hr;

    public ListPaySchemesHandler(IHrRepository hr)
    {
        _hr = hr;
    }

    public async Task<Result<List<PaySchemeDTO>>> Handle(ListPaySchemesQuery request, CancellationToken cancellationToken)
    {
        var list = await _hr.ListPaySchemesAsync(request.CompanyId, request.OnlyActive);
        return Result<List<PaySchemeDTO>>.Success(list);
    }
}

