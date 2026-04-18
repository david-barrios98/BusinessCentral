using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using BusinessCentral.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories
{
    public class CommonRepository : SqlConfigServer, ICommonRepository
    {
        public CommonRepository(IConfiguration configuration) : base(configuration)
        {
        }

        #region Geografía

        public async Task<List<CountryResponse>> ListCountriesAsync()
        {
            return await ExecuteStoredProcedureAsync(
                StoredProcedures.Common.sp_list_countries,
                null, // No requiere parámetros
                reader => SqlDataReaderMapper.MapToDto<CountryResponse>(reader));
        }

        public async Task<List<DepartmentResponse>> ListDepartmentsByCountryAsync(int countryId)
        {
            var parameters = new[]
            {
                CreateParameter("@CountryId", countryId, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureAsync(
                StoredProcedures.Common.sp_list_departments_by_country,
                parameters,
                reader => SqlDataReaderMapper.MapToDto<DepartmentResponse>(reader));
        }

        public async Task<List<CityResponse>> ListCitiesByDepartmentAsync(int departmentId)
        {
            var parameters = new[]
            {
                CreateParameter("@DepartmentId", departmentId, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureAsync(
                StoredProcedures.Common.sp_list_cities_by_department,
                parameters,
                reader => SqlDataReaderMapper.MapToDto<CityResponse>(reader));
        }

        #endregion

        #region Documentos y Configuración

        public async Task<List<DocumentTypeResponse>> ListDocumentTypesAsync()
        {
            return await ExecuteStoredProcedureAsync(
                StoredProcedures.Common.sp_list_document_types,
                null,
                reader => SqlDataReaderMapper.MapToDto<DocumentTypeResponse>(reader));
        }

        public async Task<List<MembershipPlanResponse>> ListMembershipPlansAsync()
        {
            return await ExecuteStoredProcedureAsync(
                StoredProcedures.Config.sp_list_membership_plans,
                null,
                reader => SqlDataReaderMapper.MapToDto<MembershipPlanResponse>(reader));
        }

        public async Task<MembershipPlanResponse?> GetMembershipPlanByIdAsync(int id)
        {
            var parameters = new[]
            {
                CreateParameter("@Id", id, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureSingleAsync(
                StoredProcedures.Config.sp_get_membership_plan_by_id,
                parameters,
                reader => SqlDataReaderMapper.MapToDto<MembershipPlanResponse>(reader));
        }

        #endregion
    }
}