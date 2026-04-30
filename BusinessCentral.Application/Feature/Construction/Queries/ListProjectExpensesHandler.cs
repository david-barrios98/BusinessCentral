using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.Feature.Construction.Query;
using BusinessCentral.Application.DTOs.Construction;

namespace BusinessCentral.Application.Feature.Construction.Handler
{
    public class ListProjectExpensesHandler : IRequestHandler<ListProjectExpensesQuery, Result<List<ProjectExpenseDto>>>
    {
        private readonly IProjectExpenseRepository _repo;
        public ListProjectExpensesHandler(IProjectExpenseRepository repo) => _repo = repo;

        public async Task<Result<List<ProjectExpenseDto>>> Handle(ListProjectExpensesQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListExpensesAsync(request.ProjectId, request.Page, request.PageSize);
            return Result<List<ProjectExpenseDto>>.Success(list);
        }
    }
}