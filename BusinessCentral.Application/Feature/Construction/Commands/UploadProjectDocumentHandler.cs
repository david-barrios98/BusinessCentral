using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessCentral.Application.Feature.Construction.Commands
{
    public class UploadProjectDocumentHandler : IRequestHandler<UploadProjectDocumentCommand, Result<bool>>
    {
        private readonly IProjectDocumentRepository _repo; private readonly IFileStorageService _storage; private readonly IHttpContextAccessor _http;
        public UploadProjectDocumentHandler(IProjectDocumentRepository repo, IFileStorageService storage, IHttpContextAccessor http)
        {
            _repo = repo;
            _storage = storage;
            _http = http;
        }

        public async Task<Result<bool>> Handle(UploadProjectDocumentCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _http.HttpContext?.User.FindFirst("userId")?.Value;
            int? userId = null;
            if (int.TryParse(userIdClaim, out var uid)) userId = uid;

            using var ms = new MemoryStream();
            await request.File.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;

            var path = await _storage.SaveFileAsync(ms, request.File.FileName, $"projects/{request.ProjectId}/documents");
            await _repo.InsertProjectDocumentAsync(request.ProjectId, path, request.File.FileName, request.DocumentType, userId);

            return Result<bool>.Success(true);
        }
    }
}
