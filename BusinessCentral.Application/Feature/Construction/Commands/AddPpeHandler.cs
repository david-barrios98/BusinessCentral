using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Feature.Construction.Command;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessCentral.Application.Feature.Construction.Commands
{
    public class AddPpeHandler : IRequestHandler<AddPpeCommand, Result<int?>>
    {
        private readonly IPpeRepository _repo; public AddPpeHandler(IPpeRepository repo) => _repo = repo;
        public async Task<Result<int?>> Handle(AddPpeCommand request, CancellationToken cancellationToken)
        {
            var id = await _repo.InsertPpeRecordAsync(request.ProjectId, request.Payload.UserId, request.Payload.Item ?? string.Empty, request.Payload.Notes);
            return Result<int?>.Success(id);
        }
    }
}
