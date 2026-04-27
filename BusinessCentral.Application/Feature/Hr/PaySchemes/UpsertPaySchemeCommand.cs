using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.PaySchemes;

public sealed record UpsertPaySchemeCommand(
    int CompanyId,
    string Code,
    string Name,
    string? Unit,
    bool Active
) : IRequest<Result<bool>>;

