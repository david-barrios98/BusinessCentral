using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Feature.Construction.Query;

namespace BusinessCentral.Application.Feature.Construction.Handler
{
    public class ListToolsHandler : IRequestHandler<ListToolsQuery, Result<List<ToolDto>>>
    {
        private readonly IToolRepository _repo;
        public ListToolsHandler(IToolRepository repo) => _repo = repo;

        public async Task<Result<List<ToolDto>>> Handle(ListToolsQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListToolsAsync(request.CompanyId);
            return Result<List<ToolDto>>.Success(list);
        }
    }
}