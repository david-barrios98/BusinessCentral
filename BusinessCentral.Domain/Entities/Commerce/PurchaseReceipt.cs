using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Commerce;

[Table("PurchaseReceipt", Schema = "com")]
public sealed class PurchaseReceipt
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    public long? SupplierId { get; set; }

    public DateTime ReceiptDate { get; set; }

    [MaxLength(50)]
    public string? SupplierInvoiceNumber { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "draft"; // draft/posted/void

    public decimal Total { get; set; }

    public long? DefaultToLocationId { get; set; } // opcional

    public int? CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(SupplierId))]
    public Supplier? Supplier { get; set; }
}

