using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.Feature.Construction.Command;

namespace BusinessCentral.Application.Feature.Construction.Handler
{
    public class CreateToolHandler : IRequestHandler<CreateToolCommand, Result<int?>>
    {
        private readonly IToolRepository _repo;
        public CreateToolHandler(IToolRepository repo) => _repo = repo;

        public async Task<Result<int?>> Handle(CreateToolCommand request, CancellationToken cancellationToken)
        {
            var id = await _repo.CreateToolAsync(request.CompanyId, request.Payload.Code ?? string.Empty, request.Payload.Name ?? string.Empty, request.Payload.SerialNumber);
            return Result<int?>.Success(id);
        }
    }
}