using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessCentral.Application.DTOs.Construction
{
    public class AttendanceDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public DateTime DateWorked { get; set; }
        public decimal Hours { get; set; }
    }
}
