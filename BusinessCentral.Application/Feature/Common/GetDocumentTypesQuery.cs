using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.DTOs.Common;
using MediatR;


namespace BusinessCentral.Application.Feature.Common
{
    public record GetDocumentTypesQuery() : IRequest<Result<List<DocumentTypeResponse>>>;
}
