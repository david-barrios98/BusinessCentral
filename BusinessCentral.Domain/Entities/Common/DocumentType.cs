using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessCentral.Domain.Entities.Common
{
    [Table("document_types", Schema = "common")]
    public class DocumentType
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("name")]
        public string Name { get; set; } = null!; // Ej: "Cédula de Ciudadanía"

        [Required]
        [MaxLength(10)]
        [Column("abbreviation")]
        public string Abbreviation { get; set; } = null!; // Ej: "CC"

        [Column("active")]
        public bool Active { get; set; } = true;
    }
}
