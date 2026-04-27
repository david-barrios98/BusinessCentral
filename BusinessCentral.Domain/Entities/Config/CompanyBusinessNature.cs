using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Config;

[Table("CompanyBusinessNature", Schema = "config")]
public sealed class CompanyBusinessNature
{
    public int CompanyId { get; set; }
    public int BusinessNatureId { get; set; }

    public bool IsPrimary { get; set; } = false;
    public DateTime CreatedAt { get; set; }

    public Companies? Company { get; set; }
    public BusinessNature? BusinessNature { get; set; }
}

