using MediatR;
using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Core.Application.DTOs;
using BusinessCentral.Domain.Entities.Audit;
using BusinessCentral.Application.DTOs.Auth;

namespace BusinessCentral.Application.Features.Auth.Commands.Refresh
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResponseDTO>>
    {
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly ITokenService _tokenService;
        private readonly ILoginRepository _loginRepo; // para obtener datos del usuario si lo necesitas

        public RefreshTokenHandler(
            IRefreshTokenRepository refreshRepo,
            ITokenService tokenService,
            ILoginRepository loginRepo)
        {
            _refreshRepo = refreshRepo;
            _tokenService = tokenService;
            _loginRepo = loginRepo;
        }

        public async Task<Result<LoginResponseDTO>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var existing = await _refreshRepo.GetActiveByTokenAsync(request.RefreshToken);
            if (existing == null)
            {
                return Result<LoginResponseDTO>.Failure("Refresh token inválido o expirado", "INVALID_REFRESH", "Unauthorized");
            }

            // Mapear datos del usuario desde la entidad incluida (snapshot)
            var userDto = new LoginResponseDTO
            {
                UserId = existing.UserId,
                UserName = "",
                CompanyId = existing.User.CompanyId
            };

            // 1. Generar nuevo access token
            var jwtUser = new JwtUserDto
            {
                userId = userDto.UserId,
                userName = userDto.UserName ?? string.Empty,
                companyId = userDto.CompanyId.ToString(),
                companyName = userDto.CompanyName ?? string.Empty
            };

            var newAccessToken = _tokenService.GenerateAccessToken(jwtUser);

            // Opcional: rotar refresh token
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            await _refreshRepo.RevokeAsync(existing, newRefreshToken);

            var newRefreshEntity = new RefreshToken
            {
                UserId = existing.UserId,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                CompanyId = existing.User.CompanyId,
                // JwtId / AccessTokenExpiresAt pueden llenarse si extraes los claims del token
                JwtId = null,
                AccessTokenExpiresAt = null
            };
            await _refreshRepo.AddAsync(newRefreshEntity);

            userDto.AccessToken = newAccessToken;
            userDto.RefreshToken = newRefreshToken;
            userDto.ExpiresIn = _tokenService.GetAccessTokenExpirationSeconds();
            userDto.IssuedAt = DateTime.UtcNow;

            return Result<LoginResponseDTO>.Success(userDto);
        }
    }
}