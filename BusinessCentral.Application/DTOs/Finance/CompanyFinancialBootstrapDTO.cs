namespace BusinessCentral.Application.DTOs.Finance;

public sealed class CompanyFinancialBootstrapDTO
{
    public int CompanyId { get; set; }
    /// <summary>CONSTITUTION | SANITATION | MIGRATION</summary>
    public string? StartupMode { get; set; }
    public DateTime? OperatingStartDateUtc { get; set; }
    /// <summary>NOT_STARTED | IN_PROGRESS | COMPLETED</summary>
    public string BootstrapStatus { get; set; } = "NOT_STARTED";
    public string? Notes { get; set; }
}
