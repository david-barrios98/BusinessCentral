using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.DTOs.Construction;

namespace BusinessCentral.Application.Feature.Construction.Query
{
    public class GetProjectHandler : IRequestHandler<GetProjectQuery, Result<ProjectResponseDTO>>
    {
        private readonly IConstructionRepository _repo;
        public GetProjectHandler(IConstructionRepository repo) => _repo = repo;

        public async Task<Result<ProjectResponseDTO>> Handle(GetProjectQuery request, CancellationToken cancellationToken)
        {
            var p = await _repo.GetProjectByIdAsync(request.ProjectId);
            if (p == null) return Result<ProjectResponseDTO>.Failure("Project not found", "PROJECT_NOT_FOUND", "NotFound");
            return Result<ProjectResponseDTO>.Success(p);
        }
    }
}