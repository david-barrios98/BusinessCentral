using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Config;

[Table("CompanyModule", Schema = "config")]
public sealed class CompanyModule
{
    public int CompanyId { get; set; }
    public int ModuleId { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Companies? Company { get; set; }
    public Module? Module { get; set; }
}

