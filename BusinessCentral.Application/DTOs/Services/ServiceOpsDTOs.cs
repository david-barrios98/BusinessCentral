using System.Text.Json.Serialization;

namespace BusinessCentral.Application.DTOs.Services;

public sealed class ServiceCompanySettingsDTO
{
    [JsonPropertyName("companyId")]
    public int CompanyId { get; set; }

    [JsonPropertyName("enableAgendas")]
    public bool EnableAgendas { get; set; }

    [JsonPropertyName("enableCoverage")]
    public bool EnableCoverage { get; set; }

    [JsonPropertyName("enableShifts")]
    public bool EnableShifts { get; set; }

    /// <summary>DAILY | WEEKLY</summary>
    [JsonPropertyName("shiftFrequencyType")]
    public string ShiftFrequencyType { get; set; } = "WEEKLY";

    /// <summary>Tamaño del slot sugerido para agendas (minutos).</summary>
    [JsonPropertyName("shiftSlotMinutes")]
    public int ShiftSlotMinutes { get; set; } = 30;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

public sealed class UpdateServiceCompanySettingsRequest
{
    [JsonPropertyName("enableAgendas")]
    public bool EnableAgendas { get; set; }

    [JsonPropertyName("enableCoverage")]
    public bool EnableCoverage { get; set; }

    [JsonPropertyName("enableShifts")]
    public bool EnableShifts { get; set; }

    [JsonPropertyName("shiftFrequencyType")]
    public string ShiftFrequencyType { get; set; } = "WEEKLY";

    [JsonPropertyName("shiftSlotMinutes")]
    public int ShiftSlotMinutes { get; set; } = 30;
}

public sealed class ServiceCoverageAreaDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("companyId")]
    public int CompanyId { get; set; }

    /// <summary>CITY | DEPARTMENT | COUNTRY | CUSTOM</summary>
    [JsonPropertyName("coverageType")]
    public string CoverageType { get; set; } = "CITY";

    [JsonPropertyName("countryId")]
    public int? CountryId { get; set; }

    [JsonPropertyName("departmentId")]
    public int? DepartmentId { get; set; }

    [JsonPropertyName("cityId")]
    public int? CityId { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

public sealed class UpsertServiceCoverageAreaRequest
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("coverageType")]
    public string CoverageType { get; set; } = "CITY";

    [JsonPropertyName("countryId")]
    public int? CountryId { get; set; }

    [JsonPropertyName("departmentId")]
    public int? DepartmentId { get; set; }

    [JsonPropertyName("cityId")]
    public int? CityId { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;
}

public sealed class ServiceShiftTemplateDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("companyId")]
    public int CompanyId { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Formato HH:mm:ss</summary>
    [JsonPropertyName("startTime")]
    public string StartTime { get; set; } = "08:00:00";

    /// <summary>Formato HH:mm:ss</summary>
    [JsonPropertyName("endTime")]
    public string EndTime { get; set; } = "17:00:00";

    /// <summary>DAILY | WEEKLY</summary>
    [JsonPropertyName("frequencyType")]
    public string FrequencyType { get; set; } = "WEEKLY";

    [JsonPropertyName("interval")]
    public int Interval { get; set; } = 1;

    /// <summary>Bitmask (Dom=1 ... Sáb=64). Default 127 (todos).</summary>
    [JsonPropertyName("daysOfWeekMask")]
    public int DaysOfWeekMask { get; set; } = 127;

    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

public sealed class UpsertServiceShiftTemplateRequest
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("startTime")]
    public string StartTime { get; set; } = "08:00:00";

    [JsonPropertyName("endTime")]
    public string EndTime { get; set; } = "17:00:00";

    [JsonPropertyName("frequencyType")]
    public string FrequencyType { get; set; } = "WEEKLY";

    [JsonPropertyName("interval")]
    public int Interval { get; set; } = 1;

    [JsonPropertyName("daysOfWeekMask")]
    public int DaysOfWeekMask { get; set; } = 127;

    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;
}

