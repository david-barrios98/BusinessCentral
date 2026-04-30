using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BusinessCentral.Domain.Entities.Construction
{
    [Table("PpeRecord", Schema = "construction")]
    public class PpeRecord
    {
        [Key]
        public int Id { get; set; }

        public int? ProjectId { get; set; }

        [Required]
        public int UserId { get; set; }

        [MaxLength(200)]
        public string? Item { get; set; }

        public DateTime DeliveredAt { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
