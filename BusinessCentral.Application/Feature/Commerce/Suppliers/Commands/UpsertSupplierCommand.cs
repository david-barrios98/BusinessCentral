using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Suppliers.Commands;

public sealed record UpsertSupplierCommand(
    int CompanyId,
    long? Id,
    string Name,
    string? DocumentNumber,
    string? Phone,
    string? Email,
    string? Notes,
    bool Active,
    int? PerformedByUserId
) : IRequest<Result<long>>;

