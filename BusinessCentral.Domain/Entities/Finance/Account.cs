using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Finance;

[Table("Account", Schema = "fin")]
public sealed class Account
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Code { get; set; } = string.Empty; // PUC code: 110505, 4135, etc

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1)]
    public string Nature { get; set; } = "D"; // D=Débito, C=Crédito

    public int Level { get; set; } = 1; // 1..n

    public long? ParentAccountId { get; set; }

    public bool IsAuxiliary { get; set; } = false; // si permite movimiento directo

    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(ParentAccountId))]
    public Account? Parent { get; set; }
}

