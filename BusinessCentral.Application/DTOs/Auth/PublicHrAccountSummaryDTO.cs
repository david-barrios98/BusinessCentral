namespace BusinessCentral.Application.DTOs.Auth;

public sealed class PublicHrAccountSummaryDTO
{
    public int CompanyId { get; set; }
    public int UserId { get; set; }
    public decimal TotalEarned { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalLoans { get; set; }
    public decimal Net { get; set; }
}

