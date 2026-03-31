using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;


namespace BusinessCentral.Domain.Entities.Config
{
    [Table("membership_plans", Schema = "config")]
    public class MembershipPlan
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("name")]
        public string Name { get; set; } = null!; // "Gratuito", "Estándar", "Premium"

        [Required]
       [Column("price")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("billing_cycle")]
        public string BillingCycle { get; set; } = null!; // "15_days", "monthly", "quarterly", "annual"

        [Required]
        [Column("duration_days")]
        public int DurationDays { get; set; } // 15, 30, 90, 365

        [Column("max_users")]
        public int MaxUsers { get; set; } // Límite de empleados que la empresa puede crear

        [Column("is_public")]
        public bool IsPublic { get; set; } = true; // Para ocultar planes personalizados
    }
}
