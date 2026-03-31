using BusinessCentral.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BusinessCentral.Domain.Entities.Business
{
    [Table("FacilityAddress", Schema = "business")]
    public class FacilityAddress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FacilityId { get; set; }

        [ForeignKey("FacilityId")]
        public virtual Facility Facility { get; set; } = null!;

        [Required]
        [MaxLength(250)]
        public string Address { get; set; } = null!; // El "formatted_address" de la API

        [MaxLength(100)]
        public string Alias { get; set; } = "Home"; // Ej: "Casa", "Oficina", "Mamá"

        // --- Datos de Geolocalización ---

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [MaxLength(100)]
        public string? PlaceId { get; set; } // El ID único de Google Maps/Mapbox

        // --- Relaciones Jerárquicas ---

        [Required]
        public int CityId { get; set; }

        [ForeignKey("CityId")]
        public virtual City City { get; set; } = null!;

        public bool IsMain { get; set; } = false; // Para saber cuál es la dirección principal
    }
}
