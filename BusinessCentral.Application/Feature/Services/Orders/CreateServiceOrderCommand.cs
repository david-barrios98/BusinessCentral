using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Services.Orders;

public sealed record CreateServiceOrderCommand(
    int CompanyId,
    string? VehicleType,
    string? Plate,
    string? CustomerName,
    string? FulfillmentMethodCode,
    string? FulfillmentDetails
) : IRequest<Result<long>>;

