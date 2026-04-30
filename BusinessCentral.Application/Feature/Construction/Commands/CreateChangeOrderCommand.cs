using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.DTOs.Construction;

namespace BusinessCentral.Application.Feature.Construction.Command
{
    public record CreateChangeOrderCommand(int ProjectId, ChangeOrderDto Payload) : IRequest<Result<int?>>;
}