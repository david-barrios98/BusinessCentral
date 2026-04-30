using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessCentral.Application.DTOs.Construction
{
    public class ApuItemDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Yield { get; set; }
    }
}
