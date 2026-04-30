using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BusinessCentral.Domain.Entities.Construction
{
    [Table("Tool", Schema = "construction")]
    public class Tool
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [MaxLength(100)]
        public string? Code { get; set; }

        [MaxLength(250)]
        public string? Name { get; set; }

        [MaxLength(200)]
        public string? SerialNumber { get; set; }

        public bool Active { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
