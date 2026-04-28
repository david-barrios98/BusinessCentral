using System.ComponentModel.DataAnnotations;

namespace BusinessCentral.Application.DTOs.Config;

public sealed class OnboardCompanyRequestDTO : IValidatableObject
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

    /// <summary>
    /// Varias sedes en un solo alta. Si tiene elementos, se ignoran <see cref="FacilityTypeId"/> / <see cref="FacilityName"/> legacy.
    /// </summary>
    public List<OnboardFacilityDTO>? Facilities { get; set; }

    // Facility (una sola sede; usado si <see cref="Facilities"/> está vacío)
    public int FacilityTypeId { get; set; }

    [MaxLength(200)]
    public string? FacilityName { get; set; }

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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var hasFacilities = Facilities != null && Facilities.Count > 0;
        if (hasFacilities)
        {
            for (var i = 0; i < Facilities!.Count; i++)
            {
                var f = Facilities[i];
                if (f.FacilityTypeId <= 0)
                    yield return new ValidationResult(
                        "Cada sede debe tener facilityTypeId válido.",
                        [$"{nameof(Facilities)}[{i}].{nameof(OnboardFacilityDTO.FacilityTypeId)}"]);
                if (string.IsNullOrWhiteSpace(f.Name))
                    yield return new ValidationResult(
                        "Cada sede debe tener nombre.",
                        [$"{nameof(Facilities)}[{i}].{nameof(OnboardFacilityDTO.Name)}"]);
            }
        }
        else
        {
            if (FacilityTypeId <= 0)
                yield return new ValidationResult(
                    "Indique Facilities (varias sedes) o FacilityTypeId para una sola sede.",
                    [nameof(FacilityTypeId)]);
            if (string.IsNullOrWhiteSpace(FacilityName))
                yield return new ValidationResult(
                    "Indique Facilities o FacilityName para una sola sede.",
                    [nameof(FacilityName)]);
        }
    }
}

