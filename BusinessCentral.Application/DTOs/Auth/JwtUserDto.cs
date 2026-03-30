using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Core.Application.DTOs
{
    public class JwtUserDto
    {
        public int user_id { get; set; }
        public string username { get; set; } = string.Empty; // phone, email o document
        public string role { get; set; } = string.Empty;
        public List<string> permissions { get; set; } = new List<string>();
    }
}
