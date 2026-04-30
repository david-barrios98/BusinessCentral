using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BusinessCentral.Domain.Entities.Construction
{
    [Table("ProjectExpense", Schema = "construction")]
    public class ProjectExpense
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [MaxLength(500)]
        public string? Concept { get; set; }

        public int? SpentByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
