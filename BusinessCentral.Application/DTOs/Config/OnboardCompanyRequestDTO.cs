using System.ComponentModel.DataAnnotations;

namespace BusinessCentral.Application.DTOs.Config;

public sealed class OnboardCompanyRequestDTO
{
    // Company
    [Required, MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? TradeName { get; set; }

    [MaxLength(50)]
    public string? Subdomain { get; set; }

    public int? DocumentTypeId { get; set; }
    [MaxLength(50)]
    public string? DocumentNumber { get; set; }

    [MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [Required, MaxLength(50)]
    public string BusinessNatureCode { get; set; } = string.Empty;

    // Subscription
    [Required]
    public int MembershipPlanId { get; set; }

    public DateTime? StartDateUtc { get; set; }
    public bool AutoRenew { get; set; } = true;

    // Facility
    [Required]
    public int FacilityTypeId { get; set; }

    [Required, MaxLength(200)]
    public string FacilityName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? FacilityCode { get; set; }

    [MaxLength(150)]
    public string? FacilityEmail { get; set; }

    [MaxLength(20)]
    public string? FacilityPhone { get; set; }

    // Owner user
    [Required]
    public int OwnerDocumentTypeId { get; set; }

    [Required, MaxLength(50)]
    public string OwnerDocumentNumber { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string OwnerFirstName { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string OwnerLastName { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string OwnerEmail { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string OwnerPhone { get; set; } = string.Empty;

    [Required, MinLength(8), MaxLength(100)]
    public string OwnerPassword { get; set; } = string.Empty;

    [Required]
    public int OwnerRoleId { get; set; }
}

