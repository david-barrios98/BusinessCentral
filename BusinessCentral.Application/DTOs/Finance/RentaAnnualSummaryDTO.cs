namespace BusinessCentral.Application.DTOs.Finance;

public sealed class RentaAnnualSummaryDTO
{
    public int Year { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal TaxesPaid { get; set; }
    public decimal TaxesCollected { get; set; }
    public decimal NetIncome { get; set; }
}

