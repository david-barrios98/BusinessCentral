using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BusinessCentral.Domain.Entities.Auth
{
    [Table("users", Schema = "auth")]
    public class Users
    {
        [Key]
        [JsonPropertyName("id")]
        public int id { get; set; }

        [Required]
        [MaxLength(150)]
        [JsonPropertyName("username")]
        public string username { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        [JsonPropertyName("password")]
        public string password { get; set; } = null!;

        [JsonPropertyName("create")]
        public DateTime create { get; set; }

        [JsonPropertyName("update")]
        public DateTime update { get; set; }

        [JsonPropertyName("active")]
        public bool? active { get; set; } = true;

    }

}
