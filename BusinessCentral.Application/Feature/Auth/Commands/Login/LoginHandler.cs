using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.Constants;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Core.Application.DTOs;
using BusinessCentral.Domain.Entities.Audit;
using BusinessCentral.Shared.Helper;
using MediatR;

namespace BusinessCentral.Application.Features.Auth.Commands.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, Result<LoginResponseDTO>>
    {
        private readonly ILoginRepository _repository;
        private readonly IHashPasswordService _hashService;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IFailedLoginAttemptService _failedLoginAttemptService;

        public LoginHandler(
            ILoginRepository repository,
            IHashPasswordService hashService,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IUserSessionRepository userSessionRepository,
            IFailedLoginAttemptService failedLoginAttemptService)
        {
            _repository = repository;
            _hashService = hashService;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _userSessionRepository = userSessionRepository;
            _failedLoginAttemptService = failedLoginAttemptService;
        }

        public async Task<Result<LoginResponseDTO>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var username = request.userLogin.UserName;

            // 0. Verificar si la cuenta está bloqueada por varios intentos fallidos
            if (await _failedLoginAttemptService.IsAccountLockedAsync(username))
            {
                return Result<LoginResponseDTO>.Failure(
                    "Cuenta bloqueada por múltiples intentos fallidos. Intenta más tarde.",
                    "ACCOUNT_LOCKED",
                    "Unauthorized");
            }

            var user = await _repository.GetLoginUserAsync(request);

            // 1. Validamos existencia del usuario
            if (user == null)
            {
                // Registramos intento fallido por seguridad/limitación de intentos
                await _failedLoginAttemptService.RecordFailedAttemptAsync(username);

                return Result<LoginResponseDTO>.Failure(
                    Messages.GENERAL[MessageKeys.USER_NOT_FOUND],
                    MessageKeys.USER_NOT_FOUND,
                    "NotFound");
            }

            // 2. Validamos contraseña
            if (!_hashService.Verify(request.userLogin.Password, user.Password))
            {
                // Registrar intento fallido
                await _failedLoginAttemptService.RecordFailedAttemptAsync(username);

                return Result<LoginResponseDTO>.Failure(
                    Messages.GENERAL[MessageKeys.ERROR_LOGIN],
                    MessageKeys.ERROR_LOGIN,
                    "Unauthorized");
            }

            // 3. Login exitoso: limpiar contador de intentos fallidos
            await _failedLoginAttemptService.ClearFailedAttemptsAsync(username);

            // 4. Generación de Token (igual que antes)
            JwtUserDto jwtUser = new JwtUserDto
            {
                userId = user.UserId,
                userName = user.UserName,
                companyId = user.CompanyId.ToString(),
                companyName = user.CompanyName?.ToString() ?? string.Empty
            };

            var accessToken = _tokenService.GenerateAccessToken(jwtUser);

            // Antes de crear nuevo refresh token / sesión, inhabilitamos los previos
            await _refreshTokenRepository.RevokeAllByUserAsync(user.UserId, null);
            await _userSessionRepository.CloseSessionsByUserAsync(user.UserId);

            // Crear refresh token con snapshot
            var refreshValue = _tokenService.GenerateRefreshToken();
            var refreshEntity = new RefreshToken
            {
                UserId = user.UserId,
                Token = refreshValue,
                ExpiresAt = TimeZoneHelper.ConvertToColombiaTime(DateTime.UtcNow.AddMinutes(30)),
                CreatedAt = TimeZoneHelper.GetColombiaTimeNow(),
                CompanyId = user.CompanyId
            };

            await _refreshTokenRepository.AddAsync(refreshEntity);

            // Registrar sesión
            var session = new UserSession
            {
                UserId = user.UserId,
                CompanyId = user.CompanyId,
                Platform = request.Platform,
                DeviceFingerprint = null,
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent,
                LoginAt = TimeZoneHelper.GetColombiaTimeNow(),
                IsSuccess = true
            };

            await _userSessionRepository.AddAsync(session);

            user.AccessToken = accessToken;
            user.RefreshToken = refreshValue;
            user.ExpiresIn = _tokenService.GetAccessTokenExpirationSeconds();
            user.IssuedAt = TimeZoneHelper.GetColombiaTimeNow();

            return Result<LoginResponseDTO>.Success(user);
        }
    }
}