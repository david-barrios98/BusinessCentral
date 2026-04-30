using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Construction.Command
{
    public record DeleteProjectCommand(int ProjectId) : IRequest<Result<bool>>;
}