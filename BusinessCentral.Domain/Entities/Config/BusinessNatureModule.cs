using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Config;

[Table("BusinessNatureModule", Schema = "config")]
public sealed class BusinessNatureModule
{
    public int BusinessNatureId { get; set; }
    public int ModuleId { get; set; }
    public bool IsDefaultEnabled { get; set; } = true;
    public int SortOrder { get; set; } = 0;
    public DateTime CreatedAt { get; set; }

    public BusinessNature? BusinessNature { get; set; }
    public Module? Module { get; set; }
}

