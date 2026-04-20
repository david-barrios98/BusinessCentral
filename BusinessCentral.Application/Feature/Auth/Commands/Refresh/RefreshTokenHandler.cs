using MediatR;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Core.Application.DTOs;
using BusinessCentral.Domain.Entities.Audit;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Auth.Commands.Refresh
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResponseDTO>>
    {
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly ITokenService _tokenService;
        private readonly IRedisService _redisService;


        public RefreshTokenHandler(
            IRefreshTokenRepository refreshRepo,
            ITokenService tokenService,
            IRedisService redisService)
        {
            _refreshRepo = refreshRepo;
            _tokenService = tokenService;
            _redisService = redisService;
        }

        public async Task<Result<LoginResponseDTO>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var existing = await _refreshRepo.GetActiveByTokenAsync(request.RefreshToken);

            if (existing == null || string.IsNullOrEmpty(existing.UserId.ToString()))
            {
                return Result<LoginResponseDTO>.Failure("Refresh token inválido o expirado", "INVALID_REFRESH", "Unauthorized");
            }

            var jwtUser = new JwtUserDto
            {
                userId = existing.UserId,
                userName = existing.UserName,
                companyId = existing.CompanyId.ToString(),
                companyName = existing.CompanyName,
                LoginField = existing.LoginField
            };

            // 2. Generar nuevo access token
            var newAccessToken = _tokenService.GenerateAccessToken(jwtUser);
            var newRefreshTokenValue = _tokenService.GenerateRefreshToken();

            // 3. Rotación: Revocamos tokens anteriores
            await _refreshRepo.RevokeAllByUserAsync(existing.UserSessionId, newRefreshTokenValue);

            // 4. Crear nueva entidad de Refresh Token
            var newRefreshEntity = new RefreshToken
            {
                UserSessionId = existing.UserSessionId,
                Token = newRefreshTokenValue,
                JwtId = null,
                AccessTokenExpiresAt = null
            };

            await _refreshRepo.AddAsync(newRefreshEntity);

            // 5. Preparar respuesta final
            existing.AccessToken = newAccessToken;
            existing.RefreshToken = newRefreshTokenValue;
            existing.ExpiresIn = _tokenService.GetAccessTokenExpirationSeconds();


            if (!string.IsNullOrEmpty(request.Token))
            {
                await _redisService.RevokeTokenAsync(request.Token);
            }

            return Result<LoginResponseDTO>.Success(existing);
        }
    }
}