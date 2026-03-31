using BusinessCentral.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BusinessCentral.Domain.Entities.Auth
{
    [Table("user_addresses", Schema = "auth")]
    public class UserAddress
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("userinfo_id")]
        public int UserInfoId { get; set; }

        [ForeignKey("UserInfoId")]
        public virtual UsersInfo UserInfo { get; set; } = null!;

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
