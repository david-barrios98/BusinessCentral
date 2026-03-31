using BusinessCentral.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessCentral.Domain.Entities.Business;
using BusinessCentral.Domain.Entities.Config;

namespace BusinessCentral.Domain.Entities.Auth
{
    [Table("users_info", Schema = "auth")]
    public class UsersInfo
    {
        [Key] [Column("id")] public int Id { get; set; }

        [Required] [Column("company_id")] public int CompanyId { get; set; }

        [ForeignKey("CompanyId")] public virtual Companies Company { get; set; } = null!;

        [Required]
        [Column("document_type_id")]
        public int DocumentTypeId { get; set; }

        [ForeignKey("DocumentTypeId")] public virtual DocumentType DocumentType { get; set; } = null!;

        [MaxLength(20)]
        [Column("document_number")]
        public string? DocumentNumber { get; set; }

        [MaxLength(150)]
        [Column("first_name")]
        public string? FirstName { get; set; }


        [MaxLength(150)]
        [Column("last_name")]
        public string? LastName { get; set; }


        [MaxLength(100)]
        [Column("email")]
        public string? Email { get; set; } 

        [MaxLength(255)] [Column("password")]
        public string? Password { get; set; } // Nullable por si usa Google Auth

        [Required]
        [MaxLength(10)]
        [Column("phone")]
        public string Phone { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [Column("auth_provider")]
        public string AuthProvider { get; set; } = "Local"; // "Local" o "Google"

        [MaxLength(100)]
        [Column("external_id")]
        public string? ExternalId { get; set; } // ID de Google

        [Required] [Column("role_id")] public int RoleId { get; set; }

        [ForeignKey("RoleId")] public virtual Role Role { get; set; } = null!;

        [Column("confirmed_account")] public bool ConfirmedAccount { get; set; } = false;

        [Column("active")] public bool Active { get; set; } = true;

        [Column("create")] public DateTime Created { get; set; } = DateTime.Now;

        [Column("update")] public DateTime Updated { get; set; } = DateTime.Now;

        public virtual ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();
    }
}
