using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Application.DTOs.Common
{
    public class DocumentTypeRequest
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public bool Active { get; set; }
    }

    public class DocumentTypeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public bool Active { get; set; }
    }
}
