using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Feature.Construction.Query;

namespace BusinessCentral.Application.Feature.Construction.Handler
{
    public class ListApuItemsHandler : IRequestHandler<ListApuItemsQuery, Result<List<ApuItemDto>>>
    {
        private readonly IApuRepository _repo;

        public ListApuItemsHandler(IApuRepository repo) => _repo = repo;

        public async Task<Result<List<ApuItemDto>>> Handle(ListApuItemsQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListApuItemsAsync(request.ProjectId);
            return Result<List<ApuItemDto>>.Success(list);
        }
    }
}