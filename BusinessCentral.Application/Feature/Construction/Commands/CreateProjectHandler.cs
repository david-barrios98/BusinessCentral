using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Construction.Command
{
    public class CreateProjectHandler : IRequestHandler<CreateProjectCommand, Result<int?>>
    {
        private readonly IConstructionRepository _repo;
        public CreateProjectHandler(IConstructionRepository repo) => _repo = repo;

        public async Task<Result<int?>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var id = await _repo.CreateProjectAsync(request.Project);
            return Result<int?>.Success(id);
        }
    }
}