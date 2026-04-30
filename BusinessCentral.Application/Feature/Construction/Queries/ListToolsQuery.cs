using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.DTOs.Construction;

namespace BusinessCentral.Application.Feature.Construction.Query
{
    public record ListToolsQuery(int CompanyId) : IRequest<Result<List<ToolDto>>>;
}