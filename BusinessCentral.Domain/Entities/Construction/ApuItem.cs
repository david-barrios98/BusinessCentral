using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BusinessCentral.Domain.Entities.Construction
{
    [Table("ApuItem", Schema = "construction")]
    public class ApuItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [MaxLength(100)]
        public string? Code { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Unit { get; set; }

        [Required]
        public decimal UnitPrice { get; set; } = 0m;

        [Required]
        public decimal Yield { get; set; } = 1m;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
