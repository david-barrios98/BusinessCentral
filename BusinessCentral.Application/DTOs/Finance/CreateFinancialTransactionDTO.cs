using System.ComponentModel.DataAnnotations;

namespace BusinessCentral.Application.DTOs.Finance;

public sealed class CreateFinancialTransactionDTO
{
    [Required]
    public DateTime TxDate { get; set; }

    [Required, MaxLength(20)]
    public string Direction { get; set; } = "IN"; // IN/OUT

    [Required, MaxLength(30)]
    public string Kind { get; set; } = "OPERATING"; // OPERATING/INVESTING/FINANCING/TAX

    [MaxLength(50)]
    public string? CategoryCode { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [MaxLength(50)]
    public string? ThirdPartyDocument { get; set; }

    [MaxLength(200)]
    public string? ThirdPartyName { get; set; }

    [MaxLength(50)]
    public string? SourceModule { get; set; }

    [MaxLength(50)]
    public string? ReferenceType { get; set; }

    [MaxLength(100)]
    public string? ReferenceId { get; set; }

    [MaxLength(50)]
    public string? TaxCode { get; set; }
}

