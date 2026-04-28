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
    [Table("UsersInfo", Schema = "auth")]
    public class UsersInfo
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nullable para permitir superusuarios/sistema sin compañía asociada.
        /// Para usuarios tenant debe tener valor.
        /// </summary>
        public int? CompanyId { get; set; }

        [ForeignKey("CompanyId")] 
        public virtual Companies? Company { get; set; }

        [Required]
        public int DocumentTypeId { get; set; }

        [ForeignKey("DocumentTypeId")] public virtual DocumentType DocumentType { get; set; } = null!;

        [MaxLength(20)]
        public string? DocumentNumber { get; set; }

        [MaxLength(150)]
        public string? FirstName { get; set; }


        [MaxLength(150)]
        public string? LastName { get; set; }


        [MaxLength(100)]
        public string? Email { get; set; } 

        [MaxLength(255)] 
        public string? Password { get; set; } // Nullable por si usa Google Auth

        [Required]
        [MaxLength(10)]
        public string Phone { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string AuthProvider { get; set; } = "Local"; // "Local" o "Google"

        [MaxLength(100)]
        public string? ExternalId { get; set; } // ID de Google

        [Required] public int RoleId { get; set; }

        [ForeignKey("RoleId")] public virtual Role Role { get; set; } = null!;

        public bool ConfirmedAccount { get; set; } = false;

        // Si es false: la persona existe para nómina/relaciones, pero NO puede autenticarse al sistema.
        public bool CanLogin { get; set; } = true;

        public bool Active { get; set; } = true;

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime Updated { get; set; } = DateTime.Now;

        public virtual ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();
    }
}
