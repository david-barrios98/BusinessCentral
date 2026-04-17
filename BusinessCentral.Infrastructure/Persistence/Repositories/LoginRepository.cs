using Microsoft.Extensions.Configuration;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Features.Auth.Commands.Login;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using BusinessCentral.Infrastructure.Extensions;
using BusinessCentral.Infrastructure.Persistence.Adapters;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories
{
    public class LoginRepository : SqlConfigServer, ILoginRepository
    {

        public LoginRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<LoginResponseDTO?> GetLoginUserAsync(LoginCommand request)
        {

            var parameters = new[]
            {
                CreateParameter("@UserName", request.userLogin.UserName, SqlDbType.VarChar),
                CreateParameter("@company_id", request.userLogin.CompanyId, SqlDbType.Int)
            };
            return await ExecuteStoredProcedureSingleAsync(
                StoredProcedures.Auth.sp_login_user,
                parameters,
                reader => SqlDataReaderMapper.MapToDto<LoginResponseDTO>(reader));
        }
    }
}
