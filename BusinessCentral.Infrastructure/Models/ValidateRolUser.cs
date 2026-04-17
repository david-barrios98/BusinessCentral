using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Infrastructure.Models
{
    public class ValidateRolUser
    {
        public int UserId { get; set; }
        public string? RoleName { get; set; }
        public int IsSystemRole { get; set; }
        public bool UserActive { get; set; }
        public bool RolActive { get; set; }

    }
}
