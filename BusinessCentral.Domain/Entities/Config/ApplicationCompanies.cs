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
    [Table("application_companies", Schema = "config")]
    public class ApplicationCompanies
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("company_id")]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Companies Company { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Column("application_code")]
        public string ApplicationCode { get; set; } = null!; 

        [Required]
        [MaxLength(20)]
        [Column("login_field")]
        public string LoginField { get; set; } = null!; // "email", "phone", "document"

        [Required]
        [Column("priority")]
        public int Priority { get; set; } // 1 (Principal), 2, 3...

        [Required]
        [Column("is_enabled")]
        public bool IsEnabled { get; set; } = true;

        [Column("create")]
        public DateTime Create { get; set; }

        [Column("update")]
        public DateTime Update { get; set; }

        [Column("active")]
        public bool? Active { get; set; } = true;
    }
}
