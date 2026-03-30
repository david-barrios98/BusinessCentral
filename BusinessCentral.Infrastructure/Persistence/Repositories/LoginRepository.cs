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
        private readonly BusinessCentralDbContext _context;

        public LoginRepository(IConfiguration configuration, BusinessCentralDbContext context) : base(configuration)
        {
            _context = context;
        }

        public async Task<LoginResponseDTO?> GetLoginUserAsync(LoginCommand request)
        {

            var parameters = new[]
            {
                CreateParameter("@username", request.username, SqlDbType.VarChar)
            };
            return await ExecuteStoredProcedureSingleAsync(
                StoredProcedures.Auth.sp_login_user,
                parameters,
                reader => SqlDataReaderMapper.MapToDto<LoginResponseDTO>(reader));
        }
    }
}
