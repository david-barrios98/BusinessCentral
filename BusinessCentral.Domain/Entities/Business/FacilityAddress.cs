using BusinessCentral.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BusinessCentral.Domain.Entities.Business
{
    [Table("Facility_address", Schema = "business")]
    public class FacilityAddress
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("facility_id")]
        public int FacilityId { get; set; }

        [ForeignKey("FacilityId")]
        public virtual Facility Facility { get; set; } = null!;

        [Required]
        [MaxLength(250)]
        [Column("full_address")]
        public string FullAddress { get; set; } = null!; // El "formatted_address" de la API

        [MaxLength(100)]
        [Column("alias")]
        public string Alias { get; set; } = "Home"; // Ej: "Casa", "Oficina", "Mamá"

        // --- Datos de Geolocalización ---

        [Column("latitude")]
        public double? Latitude { get; set; }

        [Column("longitude")]
        public double? Longitude { get; set; }

        [MaxLength(100)]
        [Column("place_id")]
        public string? PlaceId { get; set; } // El ID único de Google Maps/Mapbox

        // --- Relaciones Jerárquicas ---

        [Required]
        [Column("city_id")]
        public int CityId { get; set; }

        [ForeignKey("CityId")]
        public virtual City City { get; set; } = null!;

        [Column("is_main")]
        public bool IsMain { get; set; } = false; // Para saber cuál es la dirección principal
    }
}
