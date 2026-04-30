using MediatR;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.DTOs.Construction;
using Microsoft.AspNetCore.Http;
using BusinessCentral.Application.Feature.Construction.Command;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Construction.Handler
{
    public class CreateApuItemHandler : IRequestHandler<CreateApuItemCommand, Result<int?>>
    {
        private readonly IApuRepository _repo;
        private readonly IHttpContextAccessor _http;

        public CreateApuItemHandler(IApuRepository repo, IHttpContextAccessor http)
        {
            _repo = repo;
            _http = http;
        }

        public async Task<Result<int?>> Handle(CreateApuItemCommand request, CancellationToken cancellationToken)
        {
            var id = await _repo.InsertApuItemAsync(
                request.ProjectId,
                request.Item.Code,
                request.Item.Description,
                request.Item.Unit ?? string.Empty,
                request.Item.UnitPrice,
                request.Item.Yield);

            return Result<int?>.Success(id);
        }
    }
}