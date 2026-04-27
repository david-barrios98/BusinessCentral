namespace BusinessCentral.Application.DTOs.Agro;

public sealed class AgroLotKpisDTO
{
    public long LotId { get; set; }
    public int InitialUnits { get; set; }
    public int CurrentUnits { get; set; }
    public int MortalityUnits { get; set; }
    public decimal MortalityRate { get; set; }
    public decimal FeedQty { get; set; }
    public decimal FeedCost { get; set; }
}

