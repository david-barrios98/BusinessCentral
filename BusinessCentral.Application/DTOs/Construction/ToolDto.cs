using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessCentral.Application.DTOs.Construction
{
    public class ToolDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? SerialNumber { get; set; }
        public bool Active { get; set; }
    }
}
