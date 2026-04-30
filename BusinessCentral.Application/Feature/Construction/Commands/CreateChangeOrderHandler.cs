using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.DTOs.Construction;
using Microsoft.AspNetCore.Http;
using BusinessCentral.Application.Feature.Construction.Command;

namespace BusinessCentral.Application.Feature.Construction.Handler
{
    public class CreateChangeOrderHandler : IRequestHandler<CreateChangeOrderCommand, Result<int?>>
    {
        private readonly IChangeOrderRepository _repo;
        private readonly IHttpContextAccessor _http;

        public CreateChangeOrderHandler(IChangeOrderRepository repo, IHttpContextAccessor http)
        {
            _repo = repo;
            _http = http;
        }

        public async Task<Result<int?>> Handle(CreateChangeOrderCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _http.HttpContext?.User.FindFirst("userId")?.Value;
            int? userId = null;
            if (int.TryParse(userIdClaim, out var uid)) userId = uid;

            var id = await _repo.CreateChangeOrderAsync(request.ProjectId, request.Payload.Description ?? string.Empty, request.Payload.Amount, userId);
            return Result<int?>.Success(id);
        }
    }
}