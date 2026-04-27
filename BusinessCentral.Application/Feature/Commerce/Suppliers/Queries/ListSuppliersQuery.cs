using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Suppliers.Queries;

public sealed record ListSuppliersQuery(int CompanyId, bool OnlyActive = true, string? Q = null)
    : IRequest<Result<List<SupplierDTO>>>;

