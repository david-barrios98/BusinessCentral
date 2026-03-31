using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace BusinessCentral.Domain.Entities.Business
{
    [Table("facility", Schema = "business")]
    internal class Facility
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("company_id")]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Companies Company { get; set; } = null!;

        [Required]
        [Column("facility_type_id")]
        public int FacilityTypeId { get; set; }

        [ForeignKey("FacilityTypeId")]
        public virtual FacilityType FacilityType { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Column("name")]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Column("code")]
        public string Code { get; set; } = null!;

        [MaxLength(150)]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; } = null!;

        [MaxLength(20)]
        [Column("phone")]
        public string? Phone { get; set; }

        [Required]
        [Column("priority")]
        public int Priority { get; set; } // 1 (Principal), 2, 3...

        [Column("create")]
        public DateTime Create { get; set; }

        [Column("update")]
        public DateTime Update { get; set; }

        [Column("active")]
        public bool? Active { get; set; } = true;
    }
}
