using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using FluentValidation;
using MediatR;

namespace BusinessCentral.Application.Feature.Common
{
    public class GetCitiesByCityIdHandler : IRequestHandler<GetCityByIdQuery, Result<CityResponse?>>
    {
        private readonly ICommonRepository _repository;

        public GetCitiesByCityIdHandler(ICommonRepository repository) => _repository = repository;

        public async Task<Result<CityResponse?>> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
        {
            var data = await _repository.GetCityByIdAsync(request.CityId);

            if (data == null)
                return Result<CityResponse?>.Failure("Ciudad no válida o no encontrada.", "CITY_NOT_FOUND", "NotFound");

            return Result<CityResponse?>.Success(data);
        }
    }

    public class GetCitiesByCityIdValidator : AbstractValidator<GetCityByIdQuery>
    {
        public GetCitiesByCityIdValidator()
        {
            RuleFor(x => x.CityId)
                .GreaterThan(0).WithMessage("El ID de la ciudad debe ser válido.");
        }
    }
}
