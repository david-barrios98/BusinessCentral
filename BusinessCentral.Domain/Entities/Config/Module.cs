using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Domain.Entities.Config
{
    [Table("Module", Schema = "config")]
    public class Module
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? Code { get; set; } // Ej: "INV", "ACC", "HR"

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(250)]
        public string? Description { get; set; }

        public bool Active { get; set; } = true;
    }
}
