using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Construction.Query
{
    public class ListProjectsHandler : IRequestHandler<ListProjectsQuery, Result<List<ProjectResponseDTO>>>
    {
        private readonly IConstructionRepository _repo;
        public ListProjectsHandler(IConstructionRepository repo) => _repo = repo;

        public async Task<Result<List<ProjectResponseDTO>>> Handle(ListProjectsQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListProjectsAsync(request.CompanyId, request.Page, request.PageSize);
            return Result<List<ProjectResponseDTO>>.Success(list);
        }
    }
}