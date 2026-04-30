using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessCentral.Application.DTOs.Construction
{
    public class ProjectDocumentDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public string? DocumentType { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
