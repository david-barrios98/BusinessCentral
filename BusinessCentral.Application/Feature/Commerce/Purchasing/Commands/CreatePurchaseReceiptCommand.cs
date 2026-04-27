using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Purchasing.Commands;

public sealed record CreatePurchaseReceiptCommand(
    int CompanyId,
    long? SupplierId,
    DateTime ReceiptDate,
    string? SupplierInvoiceNumber,
    long? DefaultToLocationId,
    int? CreatedByUserId
) : IRequest<Result<long>>;

