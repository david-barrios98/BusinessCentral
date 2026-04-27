using BusinessCentral.Application.DTOs.Services;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Services.Orders;

public sealed record GetServiceOrderQuery(int CompanyId, long OrderId) : IRequest<Result<ServiceOrderDetailsDTO>>;

