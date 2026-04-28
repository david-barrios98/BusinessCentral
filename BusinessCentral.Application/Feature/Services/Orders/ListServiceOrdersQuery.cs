using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.DTOs.Services;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Services.Orders;

public sealed record ListServiceOrdersQuery(
    int CompanyId,
    string? Status,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<ServiceOrderDTO>>>;
