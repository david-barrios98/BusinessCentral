using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Features.Auth.Commands.Login;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface ILoginRepository
    {
        Task<LoginResponseDTO?> GetLoginUserAsync(LoginCommand request);
    }
}
