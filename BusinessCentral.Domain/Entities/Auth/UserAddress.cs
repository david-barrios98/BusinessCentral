using BusinessCentral.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BusinessCentral.Domain.Entities.Auth
{
    [Table("UserAddress", Schema = "auth")]
    public class UserAddress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserInfoId { get; set; }

        [ForeignKey("UserInfoId")]
        public virtual UsersInfo UserInfo { get; set; } = null!;

        [Required]
        [MaxLength(250)]
        public string Address { get; set; } = null!; // El "formatted_address" de la API

        [MaxLength(100)]
        public string? Alias { get; set; }

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
