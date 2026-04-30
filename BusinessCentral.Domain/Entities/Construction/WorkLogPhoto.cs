using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BusinessCentral.Domain.Entities.Construction
{
    [Table("WorkLogPhoto", Schema = "construction")]
    public class WorkLogPhoto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WorkLogId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string FilePath { get; set; } = null!;

        [MaxLength(255)]
        public string? FileName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
