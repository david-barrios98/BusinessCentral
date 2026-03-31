using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessCentral.Domain.Entities.Business;

namespace BusinessCentral.Domain.Entities.Config
{
    [Table("company_subscriptions", Schema = "config")]
    public class CompanySubscription
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("company_id")]
        public int CompanyId { get; set; }
        [ForeignKey(" CompanyId")]
        public virtual Companies Company { get; set; } = null!;

        [Required]
        [Column("plan_id")]
        public int PlanId { get; set; }
        [ForeignKey("PlanId")]
        public virtual MembershipPlan Plan { get; set; } = null!;

        [Required]
        [Column("start_date")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("end_date")]
        public DateTime EndDate { get; set; } // Fecha de vencimiento

        [Required]
        [Column("status")]
        public string Status { get; set; } = "active"; // "active", "expired", "trial", "pending_payment"

        [Required]
        [Column("auto_renew")]
        public bool AutoRenew { get; set; } = true;
    }
}
