using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Shared.Helper;
using BusinessCentral.Core.Application.DTOs;
using System.Security.Claims;

namespace BusinessCentral.Infrastructure.Security;

/// <summary>
/// Adaptador: Implementaci�n del puerto ITokenService
/// Usa JwtHelper para generar tokens
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtService _jwtHelper;

    public TokenService(JwtService jwtHelper)
    {
        _jwtHelper = jwtHelper;
    }

    public string GenerateAccessToken(JwtUserDto user)
    {
        return _jwtHelper.GenerateJwtToken(user);
    }

    public string GenerateRefreshToken()
    {
        return _jwtHelper.GenerateRefreshToken();
    }

    public int GetAccessTokenExpirationSeconds()
    {
        return _jwtHelper.GetAccessTokenExpirationSeconds();
    }

    public bool TryValidateToken(string token, out ClaimsPrincipal? principal)
    {
        return _jwtHelper.TryValidateToken(token, out principal);
    }
}