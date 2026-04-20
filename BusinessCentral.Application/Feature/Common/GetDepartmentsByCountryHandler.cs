using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using FluentValidation;
using MediatR;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Common
{
    public class GetDepartmentsByCountryHandler : IRequestHandler<GetDepartmentsByCountryQuery, Result<List<DepartmentResponse>>>
    {
        private readonly ICommonRepository _repository;
        public GetDepartmentsByCountryHandler(ICommonRepository repository) => _repository = repository;

        public async Task<Result<List<DepartmentResponse>>> Handle(GetDepartmentsByCountryQuery request, CancellationToken cancellationToken)
        {
            var data = await _repository.ListDepartmentsByCountryAsync(request.CountryId);
            return data != null
                ? Result<List<DepartmentResponse>>.Success(data)
                : Result<List<DepartmentResponse>>.Failure("No se encontraron departamentos.", "DEPARTMENTS_NOT_FOUND", "NotFound");
        }
    }

    public class GetDepartmentsByCountryValidator : AbstractValidator<GetDepartmentsByCountryQuery>
    {
        public GetDepartmentsByCountryValidator()
        {
            RuleFor(x => x.CountryId).GreaterThan(0).WithMessage("El ID del país es obligatorio.");
        }
    }
}
