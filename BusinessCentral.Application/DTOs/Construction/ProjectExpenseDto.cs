using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessCentral.Application.DTOs.Construction
{
    public class ProjectExpenseDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public decimal Amount { get; set; }
        public string? Concept { get; set; }
        public int? SpentByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
