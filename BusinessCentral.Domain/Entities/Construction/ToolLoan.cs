using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BusinessCentral.Domain.Entities.Construction
{
    [Table("ToolLoan", Schema = "construction")]
    public class ToolLoan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ToolId { get; set; }

        public int? ProjectId { get; set; }

        public int? BorrowedByUserId { get; set; }

        [Required]
        public DateTime LoanDate { get; set; } = DateTime.UtcNow;

        public DateTime? ReturnDate { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}
