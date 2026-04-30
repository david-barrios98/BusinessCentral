using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Feature.Construction.Query;

namespace BusinessCentral.Application.Feature.Construction.Handler
{
    public class ListChangeOrdersHandler : IRequestHandler<ListChangeOrdersQuery, Result<List<ChangeOrderDto>>>
    {
        private readonly IChangeOrderRepository _repo;
        public ListChangeOrdersHandler(IChangeOrderRepository repo) => _repo = repo;

        public async Task<Result<List<ChangeOrderDto>>> Handle(ListChangeOrdersQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListChangeOrdersAsync(request.ProjectId);
            return Result<List<ChangeOrderDto>>.Success(list);
        }
    }
}