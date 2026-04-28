namespace BusinessCentral.Application.DTOs.Config;

public sealed class FacilityTypeDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Active { get; set; }
    public DateTime Create { get; set; }
    public DateTime Update { get; set; }
}
