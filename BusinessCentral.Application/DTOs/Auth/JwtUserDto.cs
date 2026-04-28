using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Core.Application.DTOs
{
    public class JwtUserDto
    {
        public int userId { get; set; }
        public string userName { get; set; } = string.Empty; // phone, email o document
        public string companyId { get; set; } = string.Empty;
        public string companyName { get; set; } = string.Empty;
        public string? LoginField { get; set; } = null;

        public int roleId { get; set; } = 0;
        public string role { get; set; } = string.Empty;
        public bool isSystemRole { get; set; } = false;
        public bool isSuperUser { get; set; } = false;
        public List<string> permissions { get; set; } = new List<string>();
        public List<string> modules { get; set; } = new List<string>();
    }
}
