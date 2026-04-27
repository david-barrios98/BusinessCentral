using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.PaySchemes;

public sealed record ListPaySchemesQuery(int CompanyId, bool OnlyActive) : IRequest<Result<List<PaySchemeDTO>>>;

