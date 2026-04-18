using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Application.DTOs.Common
{
    public class DepartmentRequest
    {
        public string Name { get; set; }
        public int CountryId { get; set; }
    }

    public class DepartmentResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CountryId { get; set; }
        public string CountryName { get; set; }
    }
}
