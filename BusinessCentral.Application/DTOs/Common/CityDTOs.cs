using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Application.DTOs.Common
{
    public class CityRequest
    {
        public string Name { get; set; }
        public int DepartmentId { get; set; }
    }

    public class CityResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string CountryId { get; set; }
        public string CountryName { get; set; }
    }
}
