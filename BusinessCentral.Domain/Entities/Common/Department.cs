using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessCentral.Domain.Entities.Common
{
    [Table("Department", Schema = "common")]
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        // Relación con Countries
        [ForeignKey("CountryId")]
        public int CountryId { get; set; }

        // Navegación
        public virtual Countries Country { get; set; } = null!;
    }
}
