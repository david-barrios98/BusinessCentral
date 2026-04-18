using BusinessCentral.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface ICommonRepository
    {
        Task<List<CountryResponse>> ListCountriesAsync();
        Task<List<DepartmentResponse>> ListDepartmentsByCountryAsync(int countryId);
        Task<List<CityResponse>> ListCitiesByDepartmentAsync(int departmentId);
        Task<List<DocumentTypeResponse>> ListDocumentTypesAsync();
        Task<DocumentTypeResponse?> GetDocumentTypeByIdAsync(int id);
        Task<MembershipPlanResponse?> GetMembershipPlanByIdAsync(int id);
        Task<List<MembershipPlanResponse>> ListMembershipPlansAsync();
        Task<CityResponse?> GetCityByIdAsync(int id);
    }
}
