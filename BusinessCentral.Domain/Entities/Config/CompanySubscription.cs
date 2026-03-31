using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessCentral.Domain.Entities.Business;

namespace BusinessCentral.Domain.Entities.Config
{
    [Table("CompanySubscription", Schema = "config")]
    public class CompanySubscription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }
        [ForeignKey(" CompanyId")]
        public virtual Companies Company { get; set; } = null!;

        [Required]
        public int MembershipPlanId { get; set; }
        [ForeignKey("MembershipPlanId")]
        public virtual MembershipPlan Plan { get; set; } = null!;

        [Required]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime EndDate { get; set; } // Fecha de vencimiento

        [Required]
        public string Status { get; set; } = "active"; // "active", "expired", "trial", "pending_payment"

        [Required]
        public bool AutoRenew { get; set; } = true;
    }
}
