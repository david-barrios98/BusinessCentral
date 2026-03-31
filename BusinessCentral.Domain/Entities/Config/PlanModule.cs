using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace BusinessCentral.Domain.Entities.Config
{
    [Table("PlanModule", Schema = "config")]
    public class PlanModule
    {
        [Key]
        public int Id { get; set; }
        public int MembershipPlanId { get; set; }
        [ForeignKey("MembershipPlanId")]
        public virtual MembershipPlan MembershipPlan { get; set; } = null!;
        public int ModuleId { get; set; }
        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; } = null!;
    }
}
