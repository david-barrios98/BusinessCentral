using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;


namespace BusinessCentral.Domain.Entities.Config
{
    [Table("MembershipPlan", Schema = "config")]
    public class MembershipPlan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!; // "Gratuito", "Estándar", "Premium"

        [Required]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(20)]
        public string BillingCycle { get; set; } = null!; // "15_days", "monthly", "quarterly", "annual"

        [Required]
        public int DurationDays { get; set; } // 15, 30, 90, 365

        public int MaxUsers { get; set; } // Límite de empleados que la empresa puede crear

        public bool IsPublic { get; set; } = true; // Para ocultar planes personalizados
    }
}
