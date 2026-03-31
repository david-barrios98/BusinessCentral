using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BusinessCentral.Domain.Entities.Business
{
    [Table("facility_type", Schema = "business")]
    public class FacilityType
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("name")]
        public string Name { get; set; } = null!; // Razón Social


        [Column("create")]
        public DateTime Create { get; set; }

        [Column("update")]
        public DateTime Update { get; set; }

        [Column("active")]
        public bool? Active { get; set; } = true;
    }
}
