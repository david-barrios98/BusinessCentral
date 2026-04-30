using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.Feature.Construction.Command;

namespace BusinessCentral.Application.Feature.Construction.Handler
{
    public class LoanToolHandler : IRequestHandler<LoanToolCommand, Result<bool>>
    {
        private readonly IToolRepository _repo;

        public LoanToolHandler(IToolRepository repo) => _repo = repo;

        public async Task<Result<bool>> Handle(LoanToolCommand request, CancellationToken cancellationToken)
        {
            await _repo.LoanToolAsync(request.ToolId, request.Payload.ProjectId, request.Payload.BorrowedByUserId, request.Payload.Notes);
            return Result<bool>.Success(true);
        }
    }
}