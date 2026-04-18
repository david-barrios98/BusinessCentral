using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Application.Feature.Common
{
    public class GetDocumentTypeByIdHandler : IRequestHandler<GetDocumentTypeByIdQuery, Result<DocumentTypeResponse?>>
    {
        private readonly ICommonRepository _repository;
        public GetDocumentTypeByIdHandler(ICommonRepository repository) => _repository = repository;

        public async Task<Result<DocumentTypeResponse?>> Handle(GetDocumentTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var data = await _repository.GetDocumentTypeByIdAsync(request.Id);
            return Result<DocumentTypeResponse?>.Success(data);
        }
    }
}
