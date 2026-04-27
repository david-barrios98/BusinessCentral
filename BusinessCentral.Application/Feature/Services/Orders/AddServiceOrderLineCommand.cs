using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Services.Orders;

public sealed record AddServiceOrderLineCommand(
    int CompanyId,
    long OrderId,
    int ServiceId,
    decimal Quantity,
    decimal UnitPrice,
    int? EmployeeUserId
) : IRequest<Result<long>>;

