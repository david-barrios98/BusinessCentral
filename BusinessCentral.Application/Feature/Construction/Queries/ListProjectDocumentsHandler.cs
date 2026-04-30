using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessCentral.Application.Feature.Construction.Queries
{
    public class ListProjectDocumentsHandler : IRequestHandler<ListProjectDocumentsQuery, Result<List<ProjectDocumentDto>>>
    {
        private readonly IProjectDocumentRepository _repo; public ListProjectDocumentsHandler(IProjectDocumentRepository repo) => _repo = repo;
        public async Task<Result<List<ProjectDocumentDto>>> Handle(ListProjectDocumentsQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListProjectDocumentsAsync(request.ProjectId);
            return Result<List<ProjectDocumentDto>>.Success(list);
        }
    }
}
