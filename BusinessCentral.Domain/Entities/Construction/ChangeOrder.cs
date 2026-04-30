using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BusinessCentral.Domain.Entities.Construction
{
    [Table("ChangeOrder", Schema = "construction")]
    public class ChangeOrder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        public decimal Amount { get; set; } = 0m;

        public int? RequestedByUserId { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "PENDING";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
