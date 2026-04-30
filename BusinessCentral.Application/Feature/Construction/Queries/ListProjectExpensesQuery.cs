using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.DTOs.Construction;

namespace BusinessCentral.Application.Feature.Construction.Query
{
    public record ListProjectExpensesQuery(int ProjectId, int Page, int PageSize) : IRequest<Result<List<ProjectExpenseDto>>>;
}