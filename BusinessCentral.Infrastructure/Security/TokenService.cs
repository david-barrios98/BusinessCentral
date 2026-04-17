using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Core.Application.DTOs;
using BusinessCentral.Shared.Helper;
using BusinessCentral.Shared.Helpers;
using System.Security.Claims;

namespace BusinessCentral.Infrastructure.Security;

/// <summary>
/// Adaptador: Implementaci�n del puerto ITokenService
/// Usa JwtHelper para generar tokens
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtService _jwtHelper;
    private readonly IRedisService _redisService;


    public TokenService(JwtService jwtHelper, IRedisService redisService)
    {
        _jwtHelper = jwtHelper;
        _redisService = redisService;
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
    public async Task<bool> IsTokenExpired(string token)
    {
        bool isTokenExpired =  _jwtHelper.IsTokenExpired(token);
        if (isTokenExpired)
        {
            _redisService.RevokeTokenAsync(token, new TimeSpan(TimeZoneHelper.GetColombiaTimeNow().Ticks));
        }
        return isTokenExpired;
    }

    public Task<bool> IsTokenRevoked(string token)
    {
        return _redisService.IsTokenRevokedAsync(token);
    }
}