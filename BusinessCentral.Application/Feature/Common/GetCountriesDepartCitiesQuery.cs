using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.DTOs.Common;
using MediatR;

namespace BusinessCentral.Application.Feature.Common
{
    public record GetCountriesDepartCitiesQuery() : IRequest<Result<List<CountryResponse>>>;
    public record GetCitiesByDepartmentQuery(int DepartmentId)
        : IRequest<Result<List<CityResponse>>>;
    public record GetDepartmentsByCountryQuery(int CountryId)
        : IRequest<Result<List<DepartmentResponse>>>;

}
