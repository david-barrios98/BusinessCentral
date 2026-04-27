using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Purchasing.Commands;

public sealed record PostPurchaseReceiptCommand(int CompanyId, long ReceiptId)
    : IRequest<Result<bool>>;

