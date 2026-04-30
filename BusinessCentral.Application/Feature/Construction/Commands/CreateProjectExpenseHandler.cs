using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using Microsoft.AspNetCore.Http;
using BusinessCentral.Application.Feature.Construction.Command;

namespace BusinessCentral.Application.Feature.Construction.Handler
{
    public class CreateProjectExpenseHandler : IRequestHandler<CreateProjectExpenseCommand, Result<int?>>
    {
        private readonly IProjectExpenseRepository _repo;
        private readonly IHttpContextAccessor _http;

        public CreateProjectExpenseHandler(IProjectExpenseRepository repo, IHttpContextAccessor http)
        {
            _repo = repo;
            _http = http;
        }

        public async Task<Result<int?>> Handle(CreateProjectExpenseCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _http.HttpContext?.User.FindFirst("userId")?.Value;
            int? userId = null;
            if (int.TryParse(userIdClaim, out var uid)) userId = uid;

            var id = await _repo.InsertExpenseAsync(request.ProjectId, request.Payload.Amount, request.Payload.Concept ?? string.Empty, userId);
            return Result<int?>.Success(id);
        }
    }
}