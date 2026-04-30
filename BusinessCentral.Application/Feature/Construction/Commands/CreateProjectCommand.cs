using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Construction.Command
{
    public record CreateProjectCommand(CreateProjectDTO Project) : IRequest<Result<int?>>;
}