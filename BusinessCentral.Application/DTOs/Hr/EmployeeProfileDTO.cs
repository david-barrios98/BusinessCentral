namespace BusinessCentral.Application.DTOs.Hr;

public sealed class EmployeeProfileDTO
{
    public int UserId { get; set; }
    public int CompanyId { get; set; }
    public bool IsEmployee { get; set; }
    public bool ActiveEmployee { get; set; }
    public DateTime? HireDate { get; set; }

    public bool LodgingIncluded { get; set; }
    public string? LodgingLocation { get; set; }
    public bool MattressIncluded { get; set; }

    public string? MealPlanCode { get; set; }
    public decimal? MealUnitCost { get; set; }
}

