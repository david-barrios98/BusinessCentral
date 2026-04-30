using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.DTOs.Construction;

namespace BusinessCentral.Application.Feature.Construction.Query
{
    public record ListToolLoansQuery(int ToolId) : IRequest<Result<List<ToolLoanDto>>>;
}