using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BusinessCentral.Domain.Entities.Construction
{
    [Table("Attendance", Schema = "construction")]
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime DateWorked { get; set; }

        [Required]
        public decimal Hours { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
