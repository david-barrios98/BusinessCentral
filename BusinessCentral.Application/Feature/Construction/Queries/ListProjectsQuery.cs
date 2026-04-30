using MediatR;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Construction.Query
{
    public record ListProjectsQuery(int CompanyId, int Page, int PageSize) : IRequest<Result<List<ProjectResponseDTO>>>;
}