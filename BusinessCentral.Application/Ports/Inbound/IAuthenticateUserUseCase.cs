using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.DTOs.Common;

namespace BusinessCentral.Application.Ports.Inbound;

/// <summary>
/// Puerto de entrada: Caso de uso para autenticar usuario
/// </summary>
public interface IAuthenticateUserUseCase
{
    Task<ApiResponse<LoginResponseDTO>> ExecuteAsync(LoginRequestDTO request);
}
