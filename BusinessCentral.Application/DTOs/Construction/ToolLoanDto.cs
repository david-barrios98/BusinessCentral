using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessCentral.Application.DTOs.Construction
{
    public class ToolLoanDto
    {
        public int Id { get; set; }
        public int ToolId { get; set; }
        public int? ProjectId { get; set; }
        public int? BorrowedByUserId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string? Notes { get; set; }
    }
}
