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
    [Table("Countries", Schema = "common")]
    public class Countries
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(3)]
        public string IsoCode { get; set; } = null!; // Ej: COL, USA, MEX

        public bool Active { get; set; } = true;
    }
}
