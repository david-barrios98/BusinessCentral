using MediatR;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.Feature.Construction.Command;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Construction.Handler
{
    public class DeleteProjectHandler : IRequestHandler<DeleteProjectCommand, Result<bool>>
    {
        private readonly IConstructionRepository _repo;

        public DeleteProjectHandler(IConstructionRepository repo) => _repo = repo;

        public async Task<Result<bool>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            await _repo.DeleteProjectAsync(request.ProjectId);
            return Result<bool>.Success(true);
        }
    }
}