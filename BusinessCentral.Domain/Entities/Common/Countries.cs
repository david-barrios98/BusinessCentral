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
    [Table("countries", Schema = "common")]
    public class Countries
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(3)]
        [Column("iso_code")]
        public string IsoCode { get; set; } = null!; // Ej: COL, USA, MEX

        [Column("active")]
        public bool Active { get; set; } = true;
    }
}
