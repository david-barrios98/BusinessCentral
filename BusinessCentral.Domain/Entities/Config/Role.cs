using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Domain.Entities.Config
{
    [Table("roles", Schema = "config")]
    public class Role
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("company_id")]
        public int CompanyId { get; set; } // Cada empresa tiene sus propios roles

        [Required]
        [MaxLength(50)]
        [Column("name")]
        public string Name { get; set; } = null!; // Ej: "Administrador", "Cajero"

        [Column("is_system_role")]
        public bool IsSystemRole { get; set; } = false; // Si es un rol protegido del sistema
    }
}
