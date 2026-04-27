using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.Availability;

public sealed class UpsertEmployeeAvailabilityHandler : IRequestHandler<UpsertEmployeeAvailabilityCommand, Result<bool>>
{
    private readonly IHrRepository _hr;

    public UpsertEmployeeAvailabilityHandler(IHrRepository hr)
    {
        _hr = hr;
    }

    public async Task<Result<bool>> Handle(UpsertEmployeeAvailabilityCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<bool>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        if (request.Availability is null || request.Availability.UserId <= 0)
            return Result<bool>.Failure("UserId inválido.", "VALIDATION", "Validation");

        request.Availability.CompanyId = request.CompanyId;

        // Validación básica de slots
        foreach (var s in request.Availability.Slots ?? new List<EmployeeAvailabilitySlotDTO>())
        {
            if (s.DayOfWeek is < 0 or > 6)
                return Result<bool>.Failure("DayOfWeek inválido (0..6).", "VALIDATION", "Validation");
            if (s.EndTime <= s.StartTime)
                return Result<bool>.Failure("EndTime debe ser mayor a StartTime.", "VALIDATION", "Validation");
        }

        foreach (var e in request.Availability.Exceptions ?? new List<EmployeeAvailabilityExceptionDTO>())
        {
            if (e.DateTo < e.DateFrom)
                return Result<bool>.Failure("DateTo debe ser >= DateFrom.", "VALIDATION", "Validation");
        }

        var ok = await _hr.UpsertEmployeeAvailabilityAsync(request.CompanyId, request.Availability);
        return ok
            ? Result<bool>.Success(true)
            : Result<bool>.Failure("No se pudo guardar la disponibilidad.", "HR_AVAILABILITY_UPSERT", "Conflict");
    }
}

