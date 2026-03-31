using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BusinessCentral.Domain.Entities.Business;

namespace BusinessCentral.Domain.Entities.Config
{
    [Table("ApplicationCompanies", Schema = "config")]
    public class ApplicationCompanies
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Companies Company { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string ApplicationCode { get; set; } = null!; 

        [Required]
        [MaxLength(20)]
        public string LoginField { get; set; } = null!; // "email", "phone", "document"

        [Required]
        public int Priority { get; set; } // 1 (Principal), 2, 3...

        [Required]
        public bool IsEnabled { get; set; } = true;

        public DateTime Create { get; set; } = DateTime.UtcNow;

        public DateTime Update { get; set; } = DateTime.UtcNow;

        public bool? Active { get; set; } = true;
    }
}
