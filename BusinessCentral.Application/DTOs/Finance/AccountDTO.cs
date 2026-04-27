namespace BusinessCentral.Application.DTOs.Finance;

public sealed class AccountDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Nature { get; set; } = "D";
    public int Level { get; set; }
    public long? ParentAccountId { get; set; }
    public bool IsAuxiliary { get; set; }
    public bool Active { get; set; }
}

