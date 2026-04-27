using BusinessCentral.Application.DTOs.Services;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Services.Catalog;

public sealed record ListServicesQuery(int CompanyId, bool OnlyActive) : IRequest<Result<List<ServiceDTO>>>;

