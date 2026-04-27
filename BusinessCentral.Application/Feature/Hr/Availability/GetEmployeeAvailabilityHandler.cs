using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.Availability;

public sealed class GetEmployeeAvailabilityHandler : IRequestHandler<GetEmployeeAvailabilityQuery, Result<EmployeeAvailabilityDTO>>
{
    private readonly IHrRepository _hr;

    public GetEmployeeAvailabilityHandler(IHrRepository hr)
    {
        _hr = hr;
    }

    public async Task<Result<EmployeeAvailabilityDTO>> Handle(GetEmployeeAvailabilityQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0 || request.UserId <= 0)
            return Result<EmployeeAvailabilityDTO>.Failure("CompanyId/UserId inválido.", "VALIDATION", "Validation");

        var dto = await _hr.GetEmployeeAvailabilityAsync(request.CompanyId, request.UserId);
        if (dto is null)
        {
            // Disponibilidad es opcional: devolver perfil vacío por compatibilidad UX.
            return Result<EmployeeAvailabilityDTO>.Success(new EmployeeAvailabilityDTO
            {
                CompanyId = request.CompanyId,
                UserId = request.UserId,
                Active = true
            });
        }

        return Result<EmployeeAvailabilityDTO>.Success(dto);
    }
}

