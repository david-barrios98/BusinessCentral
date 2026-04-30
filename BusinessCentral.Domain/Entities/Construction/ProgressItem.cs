using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BusinessCentral.Domain.Entities.Construction
{
    [Table("ProgressItem", Schema = "construction")]
    public class ProgressItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = null!;

        [Required]
        public int Percentage { get; set; } = 0;

        [MaxLength(2000)]
        public string? Notes { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
