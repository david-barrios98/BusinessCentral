using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using FluentValidation;
using MediatR;

namespace BusinessCentral.Application.Feature.Common
{
    public class GetCitiesByDepartmentHandler : IRequestHandler<GetCitiesByDepartmentQuery, Result<List<CityResponse>>>
    {
        private readonly ICommonRepository _repository;

        public GetCitiesByDepartmentHandler(ICommonRepository repository) => _repository = repository;

        public async Task<Result<List<CityResponse>>> Handle(GetCitiesByDepartmentQuery request, CancellationToken cancellationToken)
        {
            var data = await _repository.ListCitiesByDepartmentAsync(request.DepartmentId);

            if (data == null)
                return Result<List<CityResponse>>.Failure("Departamento no válido o sin ciudades.", "CITIES_NOT_FOUND", "NotFound");

            return Result<List<CityResponse>>.Success(data);
        }
    }

    public class GetCitiesByDepartmentValidator : AbstractValidator<GetCitiesByDepartmentQuery>
    {
        public GetCitiesByDepartmentValidator()
        {
            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("El ID del departamento debe ser válido.");
        }
    }
}
