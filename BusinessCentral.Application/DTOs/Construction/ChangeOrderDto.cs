using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessCentral.Application.DTOs.Construction
{
    public class ChangeOrderDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
