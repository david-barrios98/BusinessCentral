using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BusinessCentral.Domain.Entities.Business
{
    [Table("FacilityType", Schema = "business")]
    public class FacilityType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!; // Razón Social


        public DateTime Create { get; set; }
        public DateTime Update { get; set; }

        public bool? Active { get; set; } = true;
    }
}
