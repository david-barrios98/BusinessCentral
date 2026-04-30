using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.Feature.Construction.Query;
using BusinessCentral.Application.DTOs.Construction;

namespace BusinessCentral.Application.Feature.Construction.Handler
{
    public class ListToolLoansHandler : IRequestHandler<ListToolLoansQuery, Result<List<ToolLoanDto>>>
    {
        private readonly IToolRepository _repo;
        public ListToolLoansHandler(IToolRepository repo) => _repo = repo;

        public async Task<Result<List<ToolLoanDto>>> Handle(ListToolLoansQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListToolLoansAsync(request.ToolId);
            return Result<List<ToolLoanDto>>.Success(list);
        }
    }
}