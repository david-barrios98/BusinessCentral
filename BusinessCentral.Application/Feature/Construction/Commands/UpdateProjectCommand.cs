using MediatR;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Construction.Command
{
    public record UpdateProjectCommand(UpdateProjectDTO Project) : IRequest<Result<bool>>;
}