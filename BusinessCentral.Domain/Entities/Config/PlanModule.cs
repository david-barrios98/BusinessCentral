using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace BusinessCentral.Domain.Entities.Config
{
    [Table("plan_modules", Schema = "config")]
    public class PlanModule
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("plan_id")]
        public int MembershipPlanId { get; set; }
        [ForeignKey("membership_plans_id")]
        public virtual MembershipPlan MembershipPlan { get; set; } = null!;
        [Column("module_id")]
        public int ModuleId { get; set; }
        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; } = null!;
    }
}
