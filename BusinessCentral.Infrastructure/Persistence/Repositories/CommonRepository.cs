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
        
        public async Task<CityResponse?> GetCityByIdAsync(int id)
        {
            var parameters = new[]
            {
                CreateParameter("@Id", id, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureSingleAsync(
                StoredProcedures.Common.sp_get_city_by_id,
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

        public async Task<DocumentTypeResponse?> GetDocumentTypeByIdAsync(int id)
        {
            var parameters = new[]
            {
                CreateParameter("@Id", id, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureSingleAsync(
                StoredProcedures.Common.sp_get_document_type_by_id,
                parameters,
                reader => SqlDataReaderMapper.MapToDto<DocumentTypeResponse>(reader));
        }

        public async Task<int> UpsertDocumentTypeAsync(int? id, DocumentTypeRequest request)
        {
            object idParam = id is > 0 ? id.Value : DBNull.Value;

            var parameters = new[]
            {
                CreateParameter("@Id", idParam, SqlDbType.Int),
                CreateParameter("@Name", request.Name, SqlDbType.NVarChar, 50),
                CreateParameter("@Abbreviation", request.Abbreviation, SqlDbType.NVarChar, 10),
                CreateParameter("@Active", request.Active, SqlDbType.Bit)
            };

            int? newId = await ExecuteStoredProcedureSingleAsync<int>(
                StoredProcedures.Common.sp_upsert_document_type,
                parameters,
                r => Convert.ToInt32(r["Id"]));

            return newId ?? 0;
        }

        public async Task DeleteDocumentTypeAsync(int id)
        {
            var parameters = new[]
            {
                CreateParameter("@Id", id, SqlDbType.Int)
            };

            await ExecuteStoredProcedureNonQueryAsync(
                StoredProcedures.Common.sp_delete_document_type,
                parameters);
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

        public async Task<int> UpsertMembershipPlanAsync(int? id, MembershipPlanRequest request)
        {
            object idParam = id is > 0 ? id.Value : DBNull.Value;

            var parameters = new[]
            {
                CreateParameter("@Id", idParam, SqlDbType.Int),
                CreateParameter("@Name", request.Name, SqlDbType.NVarChar, 50),
                CreateParameter("@Price", request.Price, SqlDbType.Decimal),
                CreateParameter("@BillingCycle", request.BillingCycle, SqlDbType.NVarChar, 20),
                CreateParameter("@DurationDays", request.DurationDays, SqlDbType.Int),
                CreateParameter("@MaxUsers", request.MaxUsers, SqlDbType.Int),
                CreateParameter("@IsPublic", request.IsPublic, SqlDbType.Bit)
            };

            int? newId = await ExecuteStoredProcedureSingleAsync<int>(
                StoredProcedures.Config.sp_upsert_membership_plan,
                parameters,
                r => Convert.ToInt32(r["Id"]));

            return newId ?? 0;
        }

        public async Task DeleteMembershipPlanAsync(int id)
        {
            var parameters = new[]
            {
                CreateParameter("@Id", id, SqlDbType.Int)
            };

            await ExecuteStoredProcedureNonQueryAsync(
                StoredProcedures.Config.sp_delete_membership_plan,
                parameters);
        }

        public async Task<List<MembershipPlanModuleResponse>> ListPlanModulesAsync(int membershipPlanId)
        {
            var parameters = new[]
            {
                CreateParameter("@MembershipPlanId", membershipPlanId, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureAsync(
                StoredProcedures.Config.sp_list_plan_modules,
                parameters,
                reader => SqlDataReaderMapper.MapToDto<MembershipPlanModuleResponse>(reader));
        }

        #endregion
    }
}