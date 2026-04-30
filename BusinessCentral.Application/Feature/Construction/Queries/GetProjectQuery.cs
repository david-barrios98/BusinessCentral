using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Construction.Query
{
    public record GetProjectQuery(int ProjectId) : IRequest<Result<ProjectResponseDTO>>;
}