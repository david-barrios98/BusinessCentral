using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Common
{
    public class GetCountriesHandler : IRequestHandler<GetCountriesDepartCitiesQuery, Result<List<CountryResponse>>>
    {
        private readonly ICommonRepository _repository;

        public GetCountriesHandler(ICommonRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<CountryResponse>>> Handle(GetCountriesDepartCitiesQuery request, CancellationToken cancellationToken)
        {
            var data = await _repository.ListCountriesAsync();

            if (data == null || !data.Any())
                return Result<List<CountryResponse>>.Failure("No se encontraron países registrados.", "NOT_FOUND", "NotFound");

            return Result<List<CountryResponse>>.Success(data);
        }
    }
}
