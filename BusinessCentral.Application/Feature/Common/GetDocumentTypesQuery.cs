using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;


namespace BusinessCentral.Application.Feature.Common
{
    public record GetDocumentTypesQuery() : IRequest<Result<List<DocumentTypeResponse>>>;
    public record GetDocumentTypeByIdQuery(int Id) : IRequest<Result<DocumentTypeResponse?>>;

}
