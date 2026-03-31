using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Domain.Entities.Config
{
    [Table("Role", Schema = "config")]
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; } // Cada empresa tiene sus propios roles

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!; // Ej: "Administrador", "Cajero"

        public bool IsSystemRole { get; set; } = false; // Si es un rol protegido del sistema
    }
}
