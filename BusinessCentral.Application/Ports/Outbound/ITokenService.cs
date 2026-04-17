using BusinessCentral.Core.Application.DTOs;
using System.Security.Claims;

namespace BusinessCentral.Application.Ports.Outbound;

/// <summary>
/// Puerto de salida: Servicio de generaci�n y validaci�n de tokens
/// </summary>
public interface ITokenService
{
    string GenerateAccessToken(JwtUserDto user);
    string GenerateRefreshToken();
    int GetAccessTokenExpirationSeconds();
    bool TryValidateToken(string token, out ClaimsPrincipal? principal);
    Task<bool> IsTokenExpired(string token);
    Task<bool> IsTokenRevoked(string token);

}