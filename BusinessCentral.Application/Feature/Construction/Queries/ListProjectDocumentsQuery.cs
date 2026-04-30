using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessCentral.Application.Feature.Construction.Queries
{
    public record ListProjectDocumentsQuery(int ProjectId) : IRequest<Result<List<ProjectDocumentDto>>>;
}
