using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Purchasing.Commands;

public sealed record AddPurchaseReceiptLineCommand(
    int CompanyId,
    long ReceiptId,
    int ProductId,
    long? VariantId,
    decimal Quantity,
    decimal UnitCost,
    long? ToLocationId,
    string? Notes
) : IRequest<Result<long>>;

