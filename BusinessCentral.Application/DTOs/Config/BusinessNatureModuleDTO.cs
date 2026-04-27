namespace BusinessCentral.Application.DTOs.Config;

public sealed class BusinessNatureModuleDTO
{
    public int BusinessNatureId { get; set; }
    public int ModuleId { get; set; }
    public string? ModuleCode { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public bool IsDefaultEnabled { get; set; }
    public int SortOrder { get; set; }
}

