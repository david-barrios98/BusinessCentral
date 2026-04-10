using MediatR;
using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.Constants;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Core.Application.DTOs;
using BusinessCentral.Domain.Entities.Audit;

namespace BusinessCentral.Application.Features.Auth.Commands.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, Result<LoginResponseDTO>>
    {
        private readonly ILoginRepository _repository;
        private readonly IHashPasswordService _hashService;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserSessionRepository _userSessionRepository;

        public LoginHandler(
            ILoginRepository repository,
            IHashPasswordService hashService,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IUserSessionRepository userSessionRepository)
        {
            _repository = repository;
            _hashService = hashService;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _userSessionRepository = userSessionRepository;
        }

        public async Task<Result<LoginResponseDTO>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetLoginUserAsync(request);

            // 1. Validamos existencia del usuario
            if (user == null)
            {
                // Usamos "NotFound" para que el Controller lance un 404
                return Result<LoginResponseDTO>.Failure(
                    Messages.GENERAL[MessageKeys.USER_NOT_FOUND],
                    MessageKeys.USER_NOT_FOUND,
                    "NotFound");
            }

            // 2. Validamos contraseña
            if (!_hashService.Verify(request.userLogin.Password, user.Password))
            {
                // Usamos "Unauthorized" para que el Controller lance un 401
                return Result<LoginResponseDTO>.Failure(
                    Messages.GENERAL[MessageKeys.ERROR_LOGIN],
                    MessageKeys.ERROR_LOGIN,
                    "Unauthorized");
            }

            // 3. Generación de Token
            JwtUserDto jwtUser = new JwtUserDto
            {
                userId = user.UserId,
                userName = user.UserName,
                companyId = user.CompanyId.ToString(),
                companyName = user.CompanyName?.ToString() ?? string.Empty
            };

            var accessToken = _tokenService.GenerateAccessToken(jwtUser);

            // 4. Generar refresh token y persistirlo
            var refreshTokenValue = _tokenService.GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.UserId,
                Token = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            // 5. Registrar sesión de usuario (AUDITORÍA)
            var session = new UserSession
            {
                UserId = user.UserId,
                CompanyId = user.CompanyId,
                Platform = "Web",
                DeviceFingerprint = null,
                IpAddress = null,
                UserAgent = null,
                LoginAt = DateTime.UtcNow,
                IsSuccess = true
            };

            await _userSessionRepository.AddAsync(session);

            // 6. Asignamos los tokens/valores al DTO de respuesta
            user.AccessToken = accessToken;
            user.RefreshToken = refreshTokenValue;
            user.ExpiresIn = _tokenService.GetAccessTokenExpirationSeconds();
            user.IssuedAt = DateTime.UtcNow;

            return Result<LoginResponseDTO>.Success(user);
        }
    }
}