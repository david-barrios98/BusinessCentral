namespace BusinessCentral.Application.DTOs.Config;

public sealed class OnboardCompanyResultDTO
{
    public bool Success { get; set; }
    public int CompanyId { get; set; }
    public int OwnerUserId { get; set; }
    public int BusinessNatureId { get; set; }
    public int MembershipPlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

