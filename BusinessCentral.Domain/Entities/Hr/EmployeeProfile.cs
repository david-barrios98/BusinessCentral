using BusinessCentral.Domain.Entities.Auth;
using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Hr;

[Table("EmployeeProfile", Schema = "hr")]
public sealed class EmployeeProfile
{
    [Key]
    public int UserId { get; set; }

    [Required]
    public int CompanyId { get; set; }

    public bool IsEmployee { get; set; } = true;
    public bool ActiveEmployee { get; set; } = true;
    public DateTime? HireDate { get; set; }

    public bool LodgingIncluded { get; set; }
    public string? LodgingLocation { get; set; }
    public bool MattressIncluded { get; set; }

    public string? MealPlanCode { get; set; }
    public decimal? MealUnitCost { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public UsersInfo? User { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }
}

