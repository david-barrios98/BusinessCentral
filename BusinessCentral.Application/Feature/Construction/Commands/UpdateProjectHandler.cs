using MediatR;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Feature.Construction.Command;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Construction.Handler
{
    public class UpdateProjectHandler : IRequestHandler<UpdateProjectCommand, Result<bool>>
    {
        private readonly IConstructionRepository _repo;

        public UpdateProjectHandler(IConstructionRepository repo) => _repo = repo;

        public async Task<Result<bool>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            await _repo.UpdateProjectAsync(request.Project);
            return Result<bool>.Success(true);
        }
    }
}