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
    [Table("department", Schema = "common")]
    public class Department
    {
        [Key]
        [Column("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = null!;

        // Relación con Countries
        [Column("country_id")]
        public int CountryId { get; set; }

        public Countries Countries { get; set; } = null!;
    }
}
