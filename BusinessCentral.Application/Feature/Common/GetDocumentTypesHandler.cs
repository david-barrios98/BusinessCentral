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
    public class GetDocumentTypesHandler : IRequestHandler<GetDocumentTypesQuery, Result<List<DocumentTypeResponse>>>
    {
        private readonly ICommonRepository _repository;
        public GetDocumentTypesHandler(ICommonRepository repository) => _repository = repository;

        public async Task<Result<List<DocumentTypeResponse>>> Handle(GetDocumentTypesQuery request, CancellationToken cancellationToken)
        {
            var data = await _repository.ListDocumentTypesAsync();
            return Result<List<DocumentTypeResponse>>.Success(data ?? new List<DocumentTypeResponse>());
        }
    }
}
