namespace BusinessCentral.Application.DTOs.Config;

public sealed class FulfillmentMethodDTO
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AppliesTo { get; set; } = "ANY";
    public string? Description { get; set; }
    public bool Active { get; set; }
    public bool? IsEnabledForCompany { get; set; }
}

