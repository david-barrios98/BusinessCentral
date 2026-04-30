using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.DTOs.Construction;

namespace BusinessCentral.Application.Feature.Construction.Command
{
    public record LoanToolCommand(int ToolId, ToolLoanDto Payload) : IRequest<Result<bool>>;
}