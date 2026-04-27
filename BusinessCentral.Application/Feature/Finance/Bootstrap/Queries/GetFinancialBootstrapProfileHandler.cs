using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Bootstrap.Queries;

public sealed class GetFinancialBootstrapProfileHandler : IRequestHandler<GetFinancialBootstrapProfileQuery, Result<CompanyFinancialBootstrapDTO>>
{
    private readonly ICompanyFinancialProfileRepository _repo;

    public GetFinancialBootstrapProfileHandler(ICompanyFinancialProfileRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<CompanyFinancialBootstrapDTO>> Handle(GetFinancialBootstrapProfileQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<CompanyFinancialBootstrapDTO>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        var dto = await _repo.GetAsync(request.CompanyId);
        if (dto is null)
            return Result<CompanyFinancialBootstrapDTO>.Failure("Empresa no encontrada.", "NOT_FOUND", "NotFound");

        return Result<CompanyFinancialBootstrapDTO>.Success(dto);
    }
}
