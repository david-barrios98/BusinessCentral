using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Feature.Construction.Handler;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessCentral.Application.Feature.Construction.Queries
{
    public class ListPpeHandler : IRequestHandler<ListPpeQuery, Result<List<PpeDto>>>
    {
        private readonly IPpeRepository _repo; public ListPpeHandler(IPpeRepository repo) => _repo = repo;
        public async Task<Result<List<PpeDto>>> Handle(ListPpeQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListPpeAsync(request.ProjectId);
            return Result<List<PpeDto>>.Success(list);
        }
    }
}
