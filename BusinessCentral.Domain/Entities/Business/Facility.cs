using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace BusinessCentral.Domain.Entities.Business
{
    [Table("Facility", Schema = "business")]
    public class Facility
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Companies Company { get; set; } = null!;

        [Required]
        public int FacilityTypeId { get; set; }

        [ForeignKey("FacilityTypeId")]
        public virtual FacilityType FacilityType { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(200)]
        public string? Code { get; set; }

        [MaxLength(150)]
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required]
        public int Priority { get; set; } // 1 (Principal), 2, 3...
        public DateTime Create { get; set; }
        public DateTime Update { get; set; }
        public bool? Active { get; set; } = true;
    }
}
