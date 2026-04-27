using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Services.Catalog;

public sealed record UpsertServiceCommand(
    int CompanyId,
    string Code,
    string Name,
    decimal BasePrice,
    bool Active
) : IRequest<Result<bool>>;

