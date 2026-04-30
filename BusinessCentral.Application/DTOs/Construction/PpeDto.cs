using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessCentral.Application.DTOs.Construction
{
    public class PpeDto
    {
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public int UserId { get; set; }
        public string? Item { get; set; }
        public DateTime DeliveredAt { get; set; }
        public string? Notes { get; set; }
    }
}
