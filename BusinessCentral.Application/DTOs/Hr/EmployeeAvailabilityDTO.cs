namespace BusinessCentral.Application.DTOs.Hr;

public sealed class EmployeeAvailabilityDTO
{
    public int CompanyId { get; set; }
    public int UserId { get; set; }
    public string? TimeZone { get; set; }
    public int? MaxServicesPerDay { get; set; }
    public bool Active { get; set; } = true;
    public string? Notes { get; set; }

    public List<EmployeeAvailabilitySlotDTO> Slots { get; set; } = new();
    public List<EmployeeAvailabilityExceptionDTO> Exceptions { get; set; } = new();
}

public sealed class EmployeeAvailabilitySlotDTO
{
    public long Id { get; set; }
    public int DayOfWeek { get; set; } // 0=Sunday..6=Saturday
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int? MaxServicesInSlot { get; set; }
    public bool Active { get; set; } = true;
}

public sealed class EmployeeAvailabilityExceptionDTO
{
    public long Id { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public bool IsAvailable { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}

