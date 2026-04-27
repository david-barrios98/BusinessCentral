namespace BusinessCentral.Application.DTOs.Config;

public sealed class ModuleDTO
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Active { get; set; }
}

