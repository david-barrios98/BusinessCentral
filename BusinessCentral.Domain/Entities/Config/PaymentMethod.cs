using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Config;

[Table("PaymentMethod", Schema = "config")]
public sealed class PaymentMethod
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Code { get; set; } = string.Empty; // CASH / CARD / TRANSFER / QR / CREDIT / OTHER

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Opcional: POS / ANY</summary>
    [MaxLength(20)]
    public string AppliesTo { get; set; } = "ANY";

    [MaxLength(300)]
    public string? Description { get; set; }

    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

