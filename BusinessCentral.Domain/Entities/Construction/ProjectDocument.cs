using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BusinessCentral.Domain.Entities.Construction
{
    [Table("ProjectDocument", Schema = "construction")]
    public class ProjectDocument
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string FilePath { get; set; } = null!;

        [MaxLength(255)]
        public string? FileName { get; set; }

        [MaxLength(100)]
        public string? DocumentType { get; set; }

        public int? UploadedByUserId { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
