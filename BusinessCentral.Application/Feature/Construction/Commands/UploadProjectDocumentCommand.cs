using BusinessCentral.Application.Feature.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessCentral.Application.Feature.Construction.Commands
{
    public record UploadProjectDocumentCommand(int ProjectId, IFormFile File, string DocumentType) : IRequest<Result<bool>>;
}
