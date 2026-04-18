using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Application.DTOs.Common
{
    public class MembershipPlanRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string BillingCycle { get; set; }
        public int DurationDays { get; set; }
        public int MaxUsers { get; set; }
        public bool IsPublic { get; set; }
    }

    public class MembershipPlanResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string BillingCycle { get; set; }
        public int DurationDays { get; set; }
        public int MaxUsers { get; set; }
        public bool IsPublic { get; set; }
    }
}
