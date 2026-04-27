namespace BusinessCentral.Application.DTOs.Hr;

public sealed class WorkLogDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public int UserId { get; set; }
    public DateTime WorkDate { get; set; }
    public int PaySchemeId { get; set; }
    public string? PaySchemeCode { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public string? ReferenceType { get; set; }
    public string? ReferenceId { get; set; }
    public string? Notes { get; set; }
}

