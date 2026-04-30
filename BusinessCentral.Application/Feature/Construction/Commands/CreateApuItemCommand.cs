using MediatR;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Construction.Command
{
    public record CreateApuItemCommand(int ProjectId, ApuItemDto Item) : IRequest<Result<int?>>;
}