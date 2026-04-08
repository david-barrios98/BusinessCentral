using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Domain.Entities.Config
{
    [Table("Permission", Schema = "config")]
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ModuleId { get; set; }

        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!; // Ej: "Crear Factura", "Eliminar Producto"

        [MaxLength(50)]
        public string? Code { get; set; } // Ej: "INVENTORY_CREATE"
    }
}
