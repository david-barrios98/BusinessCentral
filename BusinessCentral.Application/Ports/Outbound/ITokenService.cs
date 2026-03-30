using BusinessCentral.Core.Application.DTOs;

namespace BusinessCentral.Application.Ports.Outbound;

/// <summary>
/// Puerto de salida: Servicio de generaci�n y validaci�n de tokens
/// </summary>
public interface ITokenService
{
    string GenerateAccessToken(JwtUserDto user);
    string GenerateRefreshToken();
    int GetAccessTokenExpirationSeconds();
}