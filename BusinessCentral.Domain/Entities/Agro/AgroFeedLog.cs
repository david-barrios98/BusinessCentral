using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Agro;

[Table("AgroFeedLog", Schema = "agro")]
public sealed class AgroFeedLog
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public long LotId { get; set; }

    public DateTime FeedDate { get; set; }

    [Required]
    public int FeedProductId { get; set; } // insumo (concentrado)

    public long? FeedVariantId { get; set; }

    public decimal Quantity { get; set; } // kg/u

    public long? FromLocationId { get; set; }

    public decimal? UnitCost { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }
}

