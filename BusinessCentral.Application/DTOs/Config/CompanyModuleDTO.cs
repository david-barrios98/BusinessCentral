namespace BusinessCentral.Application.DTOs.Config;

public sealed class CompanyModuleDTO
{
    public int CompanyId { get; set; }
    public int ModuleId { get; set; }
    public string? ModuleCode { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}

