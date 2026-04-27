using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Finance;

[Table("FinancialTransaction", Schema = "fin")]
public sealed class FinancialTransaction
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public DateTime TxDate { get; set; } // Fecha contable

    [Required]
    [MaxLength(20)]
    public string Direction { get; set; } = "IN"; // IN/OUT

    [Required]
    [MaxLength(30)]
    public string Kind { get; set; } = "OPERATING"; // OPERATING/INVESTING/FINANCING/TAX

    [MaxLength(50)]
    public string? CategoryCode { get; set; } // ej: SALES, COGS, PAYROLL, RENT, IVA, RETEFUENTE

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public decimal Amount { get; set; } // siempre positivo; Direction define signo

    // Colombia / terceros (genérico)
    [MaxLength(50)]
    public string? ThirdPartyDocument { get; set; } // NIT/CC

    [MaxLength(200)]
    public string? ThirdPartyName { get; set; }

    // Trazabilidad hacia módulo fuente (POS, FARM, SERVICES, etc.)
    [MaxLength(50)]
    public string? SourceModule { get; set; }

    [MaxLength(50)]
    public string? ReferenceType { get; set; }

    [MaxLength(100)]
    public string? ReferenceId { get; set; }

    [MaxLength(50)]
    public string? TaxCode { get; set; } // IVA_19, RETEFUENTE_2_5, etc

    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }
}

