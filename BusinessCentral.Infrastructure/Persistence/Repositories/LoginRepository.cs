using Microsoft.Extensions.Configuration;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using BusinessCentral.Infrastructure.Extensions;
using System.Data;
using BusinessCentral.Application.Feature.Auth.Commands.Login;

namespace BusinessCentral.Infrastructure.Persistence.Repositories
{
    public class LoginRepository : SqlConfigServer, ILoginRepository
    {

        public LoginRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<LoginResponseDTO?> GetLoginUserAsync(LoginCommand request)
        {

            var appCode = request.userLogin.ApplicationCode;
            object appCodeParam = string.IsNullOrWhiteSpace(appCode)
                ? DBNull.Value
                : appCode.Trim();

            var parameters = new[]
            {
                CreateParameter("@username", request.userLogin.UserName, SqlDbType.VarChar, 150),
                CreateParameter("@company_id", request.userLogin.CompanyId, SqlDbType.Int),
                CreateParameter("@application_code", appCodeParam, SqlDbType.NVarChar, 200)
            };
            return await ExecuteStoredProcedureSingleAsync(
                StoredProcedures.Auth.sp_login_user,
                parameters,
                reader => SqlDataReaderMapper.MapToDto<LoginResponseDTO>(reader));
        }

        public async Task<LoginResponseDTO?> GetSystemLoginUserAsync(string username)
        {
            var parameters = new[]
            {
                CreateParameter("@username", username, SqlDbType.VarChar)
            };

            return await ExecuteStoredProcedureSingleAsync(
                StoredProcedures.Auth.sp_login_system_user,
                parameters,
                reader => SqlDataReaderMapper.MapToDto<LoginResponseDTO>(reader));
        }
    }
}
